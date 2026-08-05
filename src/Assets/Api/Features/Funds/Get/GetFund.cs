using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Persistence;
using Wolverine;

namespace PAS.Assets.Features.Funds.Get;

public class GetFund : IEndpoint, IWolverineHandler {
    public record Query(long Id);

    public record Result(
        long Id,
        string Name,
        string Isin,
        string Type,
        string Status,
        string Currency
    );

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/funds/{id:long}",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<Result>(request, ct);
                    return TypedResults.Ok(res);
                })
            .Produces<Result>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Funds")
            .WithName("GetFund")
            .WithDescription("Get the fund identified by 'id'.");
    }

    public async Task<Result> HandleAsync(Query query, AssetDbContext dbContext, CancellationToken ct) {
        return await dbContext.Funds
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new Result(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString(),
                x.CurrencyId.Value
            ))
            .SingleOrDefaultAsync(ct)
            ?? throw HttpException.CreateStatus404NotFound($"Fund '{query.Id}' not found.");
    }
}
