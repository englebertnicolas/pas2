using Microsoft.AspNetCore.Routing;

namespace PAS.AspNetCore.Endpoints;

public interface IEndpoint {
    void MapEndpoint(IEndpointRouteBuilder app);
}
