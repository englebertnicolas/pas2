using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PAS.Core.Results;

namespace PAS.AspNetCore;

public static class HttpResultConverter {
    private static IHttpContextAccessor? httpContextAccessor;
    
    public static void Configure(IHttpContextAccessor httpAccessor) => httpContextAccessor = httpAccessor;

    public static IResult ToHttpResult<T>(this ErrorOr<T> errorOr, Func<T, IResult> successFunc) {
        // Check if a validation error occured and was stored by FluentValidationPolicy into HTTP context
        if (httpContextAccessor != null) {
            var key = Wolverine.FluentValidationPolicy.HttpContextItemKey;
            if (HttpContext.Items[key] is ValidationResult valResult && !valResult.IsValid)
                return valResult.ToHttpResult();
        }

        // Check if arg errorOr is failure
        if (errorOr.IsFailure)
            return errorOr.Errors.ToHttpResult();

        return successFunc(errorOr.Value);
    }

    public static ProblemHttpResult ToHttpResult(this ErrorInfo error)
        => ToHttpResult([error]);

    public static ProblemHttpResult ToHttpResult(this IEnumerable<ErrorInfo> errors)
        => ToHttpResult(errors is ErrorInfo[] array ? array.AsSpan() : [.. errors]);

    public static ProblemHttpResult ToHttpResult(this ReadOnlySpan<ErrorInfo> errors) {
        ArgumentOutOfRangeException.ThrowIfZero(errors.Length, nameof(errors));
        var problem = errors[0].ToProblemDetails();
        return TypedResults.Problem(problem);
    }

    public static ProblemHttpResult ToHttpResult(this ValidationResult valResult) {
        var problem = valResult.ToValidationProblemDetails();
        return TypedResults.Problem(problem);
    }

    private static ProblemDetails ToProblemDetails(this ErrorInfo error) {
        var statusCode = error.Type switch {
            ErrorType.Unprocessable => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ProblemDetails {
            Status = statusCode,
            Title = ReasonPhrases.GetReasonPhrase(statusCode).ToSentenceCase(),
            Detail = error.Message,
        };
    }

    private static ValidationProblemDetails ToValidationProblemDetails(this ValidationResult valResult) {
        return new ValidationProblemDetails(valResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
        ) {
            Status = StatusCodes.Status400BadRequest,
            Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest).ToSentenceCase(),
            Detail = "The request contains one or more validation errors.",
            Instance = HttpContext.Request.Path
        };
    }

    static HttpContext HttpContext {
        get {
            if (httpContextAccessor == null)
                throw new InvalidOperationException($"{nameof(HttpResultConverter)} was not properly configured");
            if (httpContextAccessor.HttpContext == null)
                throw new InvalidOperationException("HTTP context not available");

            return httpContextAccessor.HttpContext;
        }
    }
}
