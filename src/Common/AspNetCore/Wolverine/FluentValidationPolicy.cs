using FluentValidation;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;
using Microsoft.AspNetCore.Http;
using Wolverine;
using Wolverine.Configuration;
using Wolverine.Runtime.Handlers;

namespace PAS.AspNetCore.Wolverine;

/// <summary>
/// A Wolverine middleware policy that automatically registers fluent validation behaviors 
/// for any message handler that has a corresponding validator in the service container.
/// </summary>
/// <remarks>
/// This policy weaves a silent validation middleware that intercepts failures and stores them 
/// within the <c>HttpContext.Items</c> collection under the <c>"WolverineMiddlewareValidationResult"</c> key.<br/>
/// Unlike the native Wolverine <c>UseFluentValidation</c> method, this approach does not throw 
/// exceptions on HTTP validation failures, preventing telemetry pollution from client-side errors.
/// (Note: This telemetry noise would natively be avoided if using <c>WolverineFx.Http</c>, 
/// but this custom policy bridges the gap for standard ASP.NET Core Minimal APIs).
/// </remarks>
public class FluentValidationPolicy : IHandlerPolicy {
    internal const string HttpContextItemKey = "WolverineMiddlewareValidationResult";

    public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container) {
        foreach (var chain in chains) {
            var messageType = chain.MessageType;
            var validatorType = typeof(IValidator<>).MakeGenericType(messageType);

            if (container.HasRegistrationFor(validatorType)) {
                // 1. Adds following line to the generated code for the handler:
                //    "var result_of_BeforeAsync = await PAS.AspNetCore.Wolverine.FluentValidationPolicy.Middleware<...>.BeforeAsync(...).ConfigureAwait(false);"
                var middlewareMethod = typeof(Middleware)
                    .GetMethod(nameof(Middleware.BeforeAsync))!
                    .MakeGenericMethod(messageType);
                var methodCall = new MethodCall(typeof(Middleware), middlewareMethod);
                chain.Middleware.Insert(0, methodCall);

                // 2. Then adds following line:
                //    "if (result_of_BeforeAsync == Wolverine.HandlerContinuation.Stop) return;"
                chain.Middleware.Insert(1, new ContinuationCheckFrame(methodCall.ReturnVariable!));
            }
        }
    }

    public class Middleware {
        public static async Task<HandlerContinuation> BeforeAsync<TMessage>(
            TMessage message,
            IValidator<TMessage> validator,
            IHttpContextAccessor httpContextAccessor
        ) {
            var context = new ValidationContext<TMessage>(message);
            var result = await validator.ValidateAsync(context);

            if (!result.IsValid) {
                if (httpContextAccessor.HttpContext != null) {
                    httpContextAccessor.HttpContext.Items[HttpContextItemKey] = result;
                    return HandlerContinuation.Stop;
                } else {
                    throw new ValidationException(result.Errors);
                }
            }

            return HandlerContinuation.Continue;
        }
    }

    internal class ContinuationCheckFrame : SyncFrame {
        private readonly Variable continuation;

        public ContinuationCheckFrame(Variable continuation) {
            this.continuation = continuation;
            uses.Add(continuation);
        }

        public override void GenerateCode(GeneratedMethod method, ISourceWriter writer) {
            writer.Write($"if ({continuation.Usage} == {typeof(HandlerContinuation).FullName}.{nameof(HandlerContinuation.Stop)}) return;");
            Next?.GenerateCode(method, writer);
        }
    }
}
