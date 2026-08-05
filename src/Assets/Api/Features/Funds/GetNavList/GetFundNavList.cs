using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.AspNetCore.Paging;
using PAS.Assets.Persistence;
using Wolverine;

namespace PAS.Assets.Features.Funds.GetNavList;

public class GetFundNavList : IEndpoint, IWolverineHandler {
    public record Query(
        long Id,
        int PageNumber = 1,
        int PageSize = 100,
        bool OrderAsc = false
    ) : IPagedQuery;

    public class QueryValidator : PagedQueryValidator<Query>;

    public record Result(IReadOnlyCollection<Result.Item> Items, bool HasNextPage) {
        public record Item(DateTime Date, double Value);
    }

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/funds/{id:long}/navs",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<Result>(request, ct);
                    return TypedResults.Ok(res);
                })
            .Produces<Result>()
            .WithTags("Funds")
            .WithName("GetFundNavList")
            .WithDescription("Get a paginated list of fund NAVs.");
    }

    public async Task<Result> HandleAsync(Query query, AssetDbContext dbContext, CancellationToken ct) {
        var items = await dbContext.Funds
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .SelectMany(x => x.Navs)
            .OrderBy(query.OrderAsc, x => x.Date)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize + 1)
            .Select(x => new Result.Item(
                x.Date,
                x.Value
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > query.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }
}
