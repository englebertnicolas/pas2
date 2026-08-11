using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Persistence;
using PAS.Domain;
using Wolverine;
using Wolverine.Attributes;

namespace PAS.Assets.Features.Funds.UpsertNav;

public class UpsertFundNav : IEndpoint, IWolverineHandler {
    public record Command(
        [FromRoute] Guid Id,
        [FromBody] Command.Body Nav
    ) {
        public record Body(DateTime Date, double Value);
    }

    public class CommandValidator : AbstractValidator<Command> {
        public CommandValidator() {
            RuleFor(x => x.Nav.Value).GreaterThan(0);
        }
    }

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/{id:Guid}/navs",
                async Task<IResult> ([AsParameters] Command request, IMessageBus bus, CancellationToken ct) => {
                    var eoResult = await bus.InvokeAsync<ErrorOr<UpsertResult>>(request, ct);
                    return eoResult.ToHttpResult(r => r == UpsertResult.Created ? TypedResults.Created() : TypedResults.NoContent());
                })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Funds")
            .WithName("UpsertFundNav")
            .WithDescription("Add or update a fund NAV.");
    }

    [Transactional]
    public async Task<ErrorOr<UpsertResult>> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var eoFundId = FundId.From(command.Id);
        if (eoFundId.IsFailure) return eoFundId.Errors;
        var fundId = eoFundId.Value;

        var fund = await dbContext.Funds
            .Include(f => f.Navs.Where(x => x.Date == command.Nav.Date))
            .SingleOrDefaultAsync(x => x.Id == fundId, ct);

        if (fund == null)
            return ErrorInfo.NotFound($"Fund '{fundId}' not found");

        return fund.UpsertNav(command.Nav.Date, command.Nav.Value);
    }
}
