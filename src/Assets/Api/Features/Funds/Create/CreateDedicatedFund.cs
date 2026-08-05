using PAS.AspNetCore.Endpoints;

namespace PAS.Assets.Features.Funds.Create;

public class CreateDedicatedFund : IEndpoint {

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/dedicated", () => TypedResults.StatusCode(StatusCodes.Status501NotImplemented))
            .ProducesProblem(StatusCodes.Status501NotImplemented)
            .WithTags("Funds")
            .WithName("CreateDedicatedFund")
            .WithDescription("Create a new dedicated fund.");
    }
}
