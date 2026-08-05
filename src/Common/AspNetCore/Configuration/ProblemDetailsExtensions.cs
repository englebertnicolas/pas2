using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PAS.AspNetCore.Configuration;

public static class ProblemDetailsExtensions {

    public static IServiceCollection AddDefaultProblemDetails(this IServiceCollection serviceProvider) {
        return serviceProvider.AddProblemDetails(options => {
            options.CustomizeProblemDetails = context => {
                if (context.ProblemDetails is HttpValidationProblemDetails validation) {
                    // The Detail property is left empty by microsoft for ValidationProblemDetails
                    // -> Adding a generic message
                    context.ProblemDetails.Detail = "The request contains one or more validation errors.";
                }
            };
        });
    }
}
