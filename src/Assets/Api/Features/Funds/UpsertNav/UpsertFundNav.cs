using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Persistence;
using PAS.Domain;
using Wolverine;
using Wolverine.Attributes;

namespace PAS.Assets.Features.Funds.UpsertNav;

public class UpsertFundNav : IEndpoint, IWolverineHandler {
    public record Command(
        [FromRoute] long Id,
        [FromBody] Command.Body Nav
    ) {
        public record Body(DateTime Date, double Value);
    }

    public class CommandValidator : AbstractValidator<Command> {
        public CommandValidator() {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Nav.Value).GreaterThan(0);
        }
    }

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/{id:long}/navs",
                async Task<IResult> ([AsParameters] Command request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<UpsertResult>(request, ct);
                    return res == UpsertResult.Created ? TypedResults.Created() : TypedResults.NoContent();
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
    public async Task<UpsertResult> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Id == command.Id, ct)
            ?? throw HttpException.CreateStatus404NotFound($"Fund '{command.Id}' not found");

        await dbContext.Funds
            .Where(f => f.Id == fund.Id)
            .Include(f => f.Navs.Where(x => x.Date == command.Nav.Date))
            .LoadAsync(ct);

        return fund.UpsertNav(command.Nav.Date, command.Nav.Value);
    }
}
