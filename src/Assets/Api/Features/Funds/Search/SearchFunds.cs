using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.AspNetCore.Paging;
using PAS.Assets.Persistence;
using Wolverine;

namespace PAS.Assets.Features.Funds.Search;

public class SearchFunds : IEndpoint, IWolverineHandler {
    public record Query(
        int PageNumber = 1,
        int PageSize = 100,
        string? NameSearch = null
    ) : IPagedQuery;

    public class QueryValidator : PagedQueryValidator<Query>;

    public record Result(IReadOnlyCollection<Result.Item> Items, bool HasNextPage) {
        public record Item(
            long Id,
            string Name,
            string Isin,
            string Type,
            string Status
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/search",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<Result>(request, ct);
                    return TypedResults.Ok(res);
                })
            .Produces<Result>()
            .WithTags("Funds")
            .WithName("SearchFunds")
            .WithDescription("Search for funds matching the specified criteria.");
    }

    public async Task<Result> HandleAsync(Query query, AssetDbContext dbContext, CancellationToken ct) {
        var items = await dbContext.Funds
            .AsNoTracking()
            .WhereSearch(x => x.Name, query.NameSearch)
            .OrderBy(x => x.Name)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize + 1)
            .Select(x => new Result.Item(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString()
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > query.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }
}
