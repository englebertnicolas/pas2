using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PAS.AspNetCore.OpenApi;

/// <summary>
/// Adds 400 & 500 operation responses with ProblemDetails schema.
/// </summary>
internal class OperationDefaultResponseTransformer : IOpenApiOperationTransformer {

    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct) {
        operation.Responses ??= [];

        // 500 response
        var internalErrorResponse = new OpenApiResponse {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>(),
        };
        internalErrorResponse.Content.Add("application/problem+json", new OpenApiMediaType {
            Schema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), null, ct)
        });
        operation.Responses.TryAdd("500", internalErrorResponse);

        // 400 response
        var badRequestResponse = new OpenApiResponse {
            Description = "Bad Request",
            Content = new Dictionary<string, OpenApiMediaType>(),
        };
        badRequestResponse.Content.Add("application/problem+json", new OpenApiMediaType {
            Schema = await context.GetOrCreateSchemaAsync(typeof(ValidationProblemDetails), null, ct)
        });
        operation.Responses.TryAdd("400", badRequestResponse);
    }
}
