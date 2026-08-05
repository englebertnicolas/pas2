using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PAS.AspNetCore.OpenApi;

/// <summary>
/// Sorts Open API document operations by path (route), then by method.
/// </summary>
internal class DocumentPathOrderTransformer : IOpenApiDocumentTransformer {

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        var sortedPaths = new OpenApiPaths();

        foreach (var path in document.Paths.OrderBy(p => p.Key)) {
            var openApiPathItem = path.Value;
            if (openApiPathItem.Operations == null) continue;

            var orderedOperations = openApiPathItem.Operations
                .OrderBy(o => o.Key.Method.ToUpper() switch {
                    "GET" => 1,
                    "PATCH" => 2,
                    "POST" => 3,
                    "PUT" => 4,
                    "DELETE" => 5,
                    _ => 6
                })
                .ToDictionary(x => x.Key, x => x.Value);

            openApiPathItem.Operations.Clear();
            foreach (var op in orderedOperations) openApiPathItem.Operations.Add(op.Key, op.Value);
            sortedPaths.Add(path.Key, openApiPathItem);
        }

        document.Paths = sortedPaths;
        return Task.CompletedTask;
    }
}
