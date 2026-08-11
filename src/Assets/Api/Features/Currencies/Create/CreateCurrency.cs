using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.AspNetCore.Endpoints;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Persistence;
using Wolverine;
using Wolverine.Attributes;

namespace PAS.Assets.Features.Currencies.Create;

public class CreateCurrency : IEndpoint, IWolverineHandler {
    public record Command(
        string Id,
        string EnglishName,
        string? Symbol = null
    );

    public class CommandValidator : AbstractValidator<Command> {
        public CommandValidator() {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.EnglishName).NotEmpty();
        }
    }

    public record Result(string Id);

    public void MapEndpoint(IEndpointRouteBuilder app) {
        app
            .MapPost("/currencies",
                async Task<IResult> (Command request, IMessageBus bus, CancellationToken ct) => {
                    var eoResult = await bus.InvokeAsync<ErrorOr<Result>>(request, ct);
                    return eoResult.ToHttpResult(r => TypedResults.Created($"/currencies/{r.Id}", r));
                })
            .Produces<Result>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Currencies")
            .WithName("CreateCurrency")
            .WithDescription("Create a new currency.");
    }

    [Transactional]
    public async Task<ErrorOr<Result>> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var eoCurrency = Currency.Create(command.Id, command.EnglishName, command.Symbol);
        if (eoCurrency.IsFailure)
            return eoCurrency.Errors;
        var currency = eoCurrency.Value;

        var idExists = await dbContext.Currencies.AnyAsync(x => x.Id == currency.Id, ct);
        if (idExists)
            return ErrorInfo.Conflict($"Currency identifier '{currency.Id}' already in use");

        dbContext.Add(currency);
        return new Result(currency.Id.Value);
    }
}
