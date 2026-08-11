using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Persistence;
using Wolverine;

namespace PAS.Assets.Features.Funds.Get;

public class GetFund : IEndpoint, IWolverineHandler {
    public record Query(Guid Id);

    public record Result(
        Guid Id,
        string Name,
        string Isin,
        string Type,
        string Status,
        string Currency
    );

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapGet("/funds/{id:Guid}",
                async ([AsParameters] Query request, IMessageBus bus, CancellationToken ct) => {
                    var eoResult = await bus.InvokeAsync<ErrorOr<Result>>(request, ct);
                    return eoResult.ToHttpResult(r => TypedResults.Ok(r));
                })
            .Produces<Result>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Funds")
            .WithName("GetFund")
            .WithDescription("Get the fund identified by 'id'.");
    }

    public async Task<ErrorOr<Result>> HandleAsync(Query query, AssetDbContext dbContext, CancellationToken ct) {
        var eoFundId = FundId.From(query.Id);
        if (eoFundId.IsFailure) return eoFundId.Errors;
        var fundId = eoFundId.Value;

        var res = await dbContext.Funds
            .AsNoTracking()
            .Where(x => x.Id == fundId)
            .Select(x => new Result(
                x.Id.Value,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString(),
                x.CurrencyId.Value
            ))
            .SingleOrDefaultAsync(ct);

        if (res == null)
            return ErrorInfo.NotFound($"Fund '{query.Id}' not found.");
        return res;
    }
}
