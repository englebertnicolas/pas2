using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PAS.AspNetCore.OpenApi;
using Scalar.AspNetCore;

namespace PAS.AspNetCore.Configuration;

public static class OpenApiExtensions {

    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection serviceProvider) {
        return serviceProvider.AddOpenApi(options => {
            options.CreateSchemaReferenceId = SchemaReferenceIdHelper.CreateSchemaReferenceId;
            options
                .AddOperationTransformer<OperationDefaultResponseTransformer>()
                .AddOperationTransformer<OperationParameterCamelCaseTransformer>()
                .AddDocumentTransformer<DocumentPathOrderTransformer>();
        });
    }

    public static WebApplication UseDefaultOpenApi(this WebApplication app, string? title = null) {
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
            app.MapScalarApiReference(options => {
                if (!string.IsNullOrWhiteSpace(title))
                    options.Title = title;
                options.SortTagsAlphabetically();
            });
        }

        return app;
    }
}
