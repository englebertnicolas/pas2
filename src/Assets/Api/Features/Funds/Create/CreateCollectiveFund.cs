using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Persistence;
using Wolverine;
using Wolverine.Attributes;

namespace PAS.Assets.Features.Funds.Create;

public class CreateCollectiveFund : IEndpoint, IWolverineHandler {
    public record Command(
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

    public record Result(long Id);

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/funds/collective",
                async (Command request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<Result>(request, ct);
                    return TypedResults.Created($"/funds/{res.Id}", res);
                })
            .Produces<Result>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Funds")
            .WithName("CreateCollectiveFund")
            .WithDescription("Create a new collective fund.");
    }

    [Transactional]
    public async Task<Result> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var isin = Isin.Create(command.Isin);
        var currencyId = CurrencyId.Create(command.Currency);

        var navs = (IEnumerable<FundNav>?)null;
        if (command.Nav != null)
            navs = [FundNav.Create(command.Nav.Date, command.Nav.Value)];

        var fund = Fund.CreateCollectiveFund(FundStatus.Active, command.Name, isin, currencyId, navs);

        var existingFunds = await dbContext.Funds
           .Select(f => new { f.Name, f.Isin })
           .Where(f => f.Name == command.Name || f.Isin == isin)
           .ToListAsync(ct);

        if (existingFunds.Any(x => x.Name == command.Name))
            throw HttpException.CreateStatus409Conflict($"Fund name '{command.Name}' already in use");

        if (existingFunds.Any(x => x.Isin == isin))
            throw HttpException.CreateStatus409Conflict($"Fund isin '{command.Isin}' already in use");

        var currencyExists = await dbContext.Currencies.AnyAsync(x => x.Id == currencyId, ct);
        if (!currencyExists)
            throw HttpException.CreateStatus422Unprocessable($"Currency '{currencyId}' not found");

        await dbContext.Funds.AddAsync(fund, ct);
        return new Result(fund.Id);
    }
}
