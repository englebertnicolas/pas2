using System.Reflection;
using Microsoft.AspNetCore.Builder;
using PAS.AspNetCore.Endpoints;

namespace PAS.AspNetCore.Configuration;

public static class EndpointMappingExtensions {

    public static WebApplication MapEndpointFromAssembly(this WebApplication app, Assembly assembly) {
        var endpointTypes = assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes) {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;

            //// Convention:
            //// - Endpoint group name = last namespace segment
            //// - Endpoint name = name of the class
            //var groupName = type.Namespace?.Split('.').Last() ?? "Default";
            //var endpointName = type.Name;

            //var group = app.MapGroup(string.Empty)
            //    .WithTags(groupName)
            //    .WithMetadata(new EndpointNameMetadata(endpointName));

            //endpoint.MapEndpoint(group);

            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
