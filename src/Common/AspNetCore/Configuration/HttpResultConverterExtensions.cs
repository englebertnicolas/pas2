using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PAS.AspNetCore.Configuration;

public static partial class HttpResultConverterExtensions {

    /// <summary>
    /// Configures ErrorOr to HttpResult converter.
    /// </summary>
    public static WebApplication ConfigureHttpResultConverter(this WebApplication app) {
        var accessor = app.Services.GetRequiredService<IHttpContextAccessor>();
        HttpResultConverter.Configure(accessor);
        return app;
    }
}
