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
                async (Command request, IMessageBus bus, CancellationToken ct) => {
                    var res = await bus.InvokeAsync<Result>(request, ct);
                    return TypedResults.Created($"/currencies/{res.Id}", res);
                })
            .Produces<Result>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Currencies")
            .WithName("CreateCurrency")
            .WithDescription("Create a new currency.");
    }

    [Transactional]
    public async Task<Result> HandleAsync(Command command, AssetDbContext dbContext, CancellationToken ct) {
        var id = CurrencyId.Create(command.Id);
        var symbol = CurrencySymbol.CreateOrNull(command.Symbol);
        var currency = Currency.Create(id, command.EnglishName, symbol);

        var idExists = await dbContext.Currencies.AnyAsync(x => x.Id == id, ct);
        if (idExists)
            throw HttpException.CreateStatus409Conflict($"Currency identifier '{id}' already in use");

        dbContext.Add(currency);
        return new(currency.Id.Value);
    }
}
