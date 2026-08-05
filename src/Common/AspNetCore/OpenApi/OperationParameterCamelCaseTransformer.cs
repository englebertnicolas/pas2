using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PAS.AspNetCore.OpenApi;

/// <summary>
/// Converts the operation parameter names to camelCase to match the default JSON serialization naming policy.
/// </summary>
internal class OperationParameterCamelCaseTransformer : IOpenApiOperationTransformer {

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct) {
        if (operation.Parameters != null) {
            foreach (var parameter in operation.Parameters) {
                if (parameter is OpenApiParameter concreteParameter && concreteParameter.Name != null) {
                    concreteParameter.Name = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(concreteParameter.Name);
                }
            }
        }
        return Task.CompletedTask;
    }
}
