using FluentValidation;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Features.Funds.Search;
using Wolverine;

namespace PAS.Assets.Features.Funds.GetList;

public class GetFundList : IEndpoint {
    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/funds",
                async (IMessageBus bus, int pageNumber = 1, int pageSize = 100, CancellationToken ct = default) => {
                    // Use FundsSearch handler
                    var searchRequest = new SearchFunds.Query(pageNumber, pageSize);
                    var res = await bus.InvokeAsync<SearchFunds.Result>(searchRequest, ct);
                    return TypedResults.Ok(res);
                })
            .Produces<SearchFunds.Result>()
            .WithTags("Funds")
            .WithName("GetFundList")
            .WithDescription("Get a paginated list of funds.");
    }
}
