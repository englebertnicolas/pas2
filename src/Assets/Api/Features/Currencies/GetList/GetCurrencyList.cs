using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.AspNetCore.Paging;
using PAS.Assets.Persistence;
using Wolverine;

namespace PAS.Assets.Features.Currencies.GetList;

public class GetCurrencyList : IEndpoint, IWolverineHandler {
    public record Query(
        int PageNumber = 1,
        int PageSize = 100
    ) : IPagedQuery;

    public class QueryValidator : PagedQueryValidator<Query>;

    public record Result(IReadOnlyCollection<Result.Item> Items, bool HasNextPage) {
        public record Item(string Id, string EnglishName, string Symbol);
    }

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/currencies",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct) => {
                    var eoResult = await bus.InvokeAsync<ErrorOr<Result>>(request, ct);
                    return eoResult.ToHttpResult(r => TypedResults.Ok(r));
                })
            .Produces<Result>()
            .WithTags("Currencies")
            .WithName("GetCurrencyList")
            .WithDescription("Get a paginated list of currencies.");
    }

    public async Task<ErrorOr<Result>> HandleAsync(Query query, AssetDbContext dbContext, CancellationToken ct) {
        var items = await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize + 1)
            .Select(x => new Result.Item(
                x.Id.Value,
                x.EnglishName,
                x.Symbol.Value
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > query.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new Result(items, hasNextPage);
    }
}
