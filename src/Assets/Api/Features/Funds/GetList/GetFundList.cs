using FluentValidation;
using PAS.AspNetCore.Endpoints;
using PAS.AspNetCore.Paging;
using PAS.Assets.Features.Funds.Search;
using Wolverine;

namespace PAS.Assets.Features.Funds.GetList;

public class GetFundList : IEndpoint {
    public record Query(
        int PageNumber = 1,
        int PageSize = 100
    ) : IPagedQuery;

    public class QueryValidator : PagedQueryValidator<Query>;

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/funds",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct = default) => {
                    // Using FundsSearch handler
                    var searchRequest = new SearchFunds.Query(request.PageNumber, request.PageSize);
                    var eoResult = await bus.InvokeAsync<ErrorOr<SearchFunds.Result>>(searchRequest, ct);
                    return eoResult.ToHttpResult(r => TypedResults.Ok(r));
                })
            .Produces<SearchFunds.Result>()
            .WithTags("Funds")
            .WithName("GetFundList")
            .WithDescription("Get a paginated list of funds.");
    }
}
