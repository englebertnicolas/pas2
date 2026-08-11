using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PAS.AspNetCore.Diagnostics;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env) : IExceptionHandler {

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct) {

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier; // TraceId (standard RFC 7807)

        httpContext.Response.ContentType = "application/problem+json";

        switch (exception) {
            case BadHttpRequestException badHttpRequestException:
                var badReqProblem = new HttpValidationProblemDetails(new Dictionary<string, string[]> {
                    { "", [badHttpRequestException.Message] }
                }) {
                    Status = StatusCodes.Status400BadRequest,
                    Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest).ToSentenceCase(),
                    Detail = "The request could not be parsed correctly.",
                    Instance = httpContext.Request.Path
                };
                badReqProblem.Extensions["traceId"] = traceId;

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(badReqProblem, ct);
                return true;

            case FluentValidation.ValidationException validationEx:
                var validationProblem = new HttpValidationProblemDetails(validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                ) {
                    Status = StatusCodes.Status400BadRequest,
                    Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest).ToSentenceCase(),
                    Detail = "The request contains one or more validation errors.",
                    Instance = httpContext.Request.Path
                };
                validationProblem.Extensions["traceId"] = traceId;

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(validationProblem, ct);
                return true;

            case HttpRequestException httpException when httpException.StatusCode.HasValue:
                var httpProblem = new ProblemDetails {
                    Status = (int)httpException.StatusCode,
                    Title = ReasonPhrases.GetReasonPhrase((int)httpException.StatusCode).ToSentenceCase(),
                    Detail = httpException.Message,
                    Instance = httpContext.Request.Path
                };
                httpProblem.Extensions["traceId"] = traceId;

                httpContext.Response.StatusCode = (int)httpException.StatusCode;
                await httpContext.Response.WriteAsJsonAsync(httpProblem, ct);
                return true;

            default:
                logger.LogError(exception, "An unhandled exception occurred. TraceId: {TraceId}", traceId);

                var internalProblem = new ProblemDetails {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError).ToSentenceCase(),
                    Detail = env.IsDevelopment() ? exception.Message : "An unexpected error occurred on the server.",
                    Instance = httpContext.Request.Path
                };
                internalProblem.Extensions["traceId"] = traceId;

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(internalProblem, ct);
                return true;
        }
    }
}
