using PAS.Domain;

namespace PAS.Assets.Domain.CurrencyAggregate;

public record CurrencySymbol : ValueObject {
    public string Value { get; }

    private CurrencySymbol(string value) {
        Value = value;
    }

    public static CurrencySymbol? CreateOrNull(string? value) {
        if (value == null) return null;
        return Create(value);
    }

    public static CurrencySymbol Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Invalid currency symbol.", nameof(CurrencySymbol));

        var cleanedValue = value.Trim();
        if (cleanedValue.Length < 1 || cleanedValue.Length > 3)
            throw new DomainException("Currency symbol must be between 1 and 3 characters long.", nameof(CurrencySymbol));

        return new CurrencySymbol(cleanedValue);
    }

    public override string ToString() { return Value; }
}
