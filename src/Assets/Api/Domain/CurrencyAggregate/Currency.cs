using PAS.Domain;

namespace PAS.Assets.Domain.CurrencyAggregate;

public class Currency : Entity<CurrencyId>, IAggregateRoot {
    public string EnglishName { get; private set; } = null!;
    public CurrencySymbol Symbol { get; private set; } = null!;

    private Currency() {
        // For EF hydration
    }

    private Currency(CurrencyId id, string englishName, CurrencySymbol symbol) {
        Id = id;
        EnglishName = englishName;
        Symbol = symbol;
    }

    public static ErrorOr<Currency> Create(string id, string englishName, string? symbol) {
        var eoCurrencyId = CurrencyId.From(id);
        if (eoCurrencyId.IsFailure)
            return eoCurrencyId.Errors;

        var eoCurrencySymbol = CurrencySymbol.Create(symbol ?? id);
        if (eoCurrencySymbol.IsFailure)
            return eoCurrencySymbol.Errors;

        if (string.IsNullOrWhiteSpace(englishName))
            return ErrorInfo.Unprocessable("Invalid currency name.");

        return new Currency(eoCurrencyId.Value, englishName, eoCurrencySymbol.Value);
    }
}
