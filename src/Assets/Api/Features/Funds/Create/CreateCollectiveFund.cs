using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Persistence;
using Wolverine;
using Wolverine.Attributes;

namespace PAS.Assets.Features.Funds.Create;

public class CreateCollectiveFund : IEndpoint, IWolverineHandler {
    public record Command(
        Guid? Id,
        string Name,
        string Isin,
        string Currency,
        Command.FundNav? Nav = null
    ) {
        public record FundNav(DateTime Date, double Value);
    }

    public class CommandValidator : AbstractValidator<Command> {
        public CommandValidator() {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Isin).NotEmpty().MaximumLength(12);
            RuleFor(x => x.Currency).NotEmpty().Length(3);

            When(x => x.Nav != null, () => {
                RuleFor(x => x.Nav!.Value).GreaterThan(0);
            });
        }
    }

    public record Result(Guid Id);

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/collective",
                async (Command request, IMessageBus bus, CancellationToken ct) => {
                    var eoResult = await bus.InvokeAsync<ErrorOr<Result>>(request, ct);
                    return eoResult.ToHttpResult(r => TypedResults.Created($"/funds/{r.Id}", r));
                })
            .Produces<Result>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Funds")
            .WithName("CreateCollectiveFund")
            .WithDescription("Create a new collective fund.");
    }

    [Transactional]
    public async Task<ErrorOr<Result>> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var navs = (FundNav[]?)null;
        if (command.Nav != null) {
            var eoNav = FundNav.Create(command.Nav.Date, command.Nav.Value);
            if (eoNav.IsFailure) return eoNav.Errors;
            navs = [eoNav.Value];
        }

        var eoNewFund = Fund.CreateCollectiveFund(command.Id, FundStatus.Active, command.Name, command.Isin, command.Currency, navs);
        if (eoNewFund.IsFailure) return eoNewFund.Errors;
        var newFund = eoNewFund.Value;

        var existingFunds = await dbContext.Funds
           .Select(f => new { f.Name, f.Isin })
           .Where(f => f.Name == newFund.Name || f.Isin == newFund.Isin)
           .ToListAsync(ct);

        if (existingFunds.Any(x => x.Name == command.Name))
            return ErrorInfo.Conflict($"Fund name '{command.Name}' already in use");

        if (existingFunds.Any(x => x.Isin == newFund.Isin))
            return ErrorInfo.Conflict($"Fund isin '{command.Isin}' already in use");

        var currencyExists = await dbContext.Currencies.AnyAsync(x => x.Id == newFund.CurrencyId, ct);
        if (!currencyExists)
            return ErrorInfo.Unprocessable($"Currency '{newFund.CurrencyId}' not found");

        await dbContext.Funds.AddAsync(newFund, ct);
        return new Result(newFund.Id.Value);
    }
}
