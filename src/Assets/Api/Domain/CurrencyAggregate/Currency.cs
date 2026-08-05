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

    public static Currency Create(CurrencyId id, string englishName, CurrencySymbol? symbol) {
        symbol ??= CurrencySymbol.Create(id.Value);

        if (string.IsNullOrWhiteSpace(englishName))
            throw new DomainException("Invalid currency name.", nameof(EnglishName));

        return new(id, englishName, symbol);
    }
}
