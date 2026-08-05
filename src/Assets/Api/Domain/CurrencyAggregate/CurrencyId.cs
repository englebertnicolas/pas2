using PAS.Domain;

namespace PAS.Assets.Domain.CurrencyAggregate;

public record CurrencyId : ValueObject {
    public string Value { get; }

    private CurrencyId(string value) {
        Value = value;
    }

    public static CurrencyId Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Invalid currency code.", nameof(CurrencyId));

        var cleanedValue = value.Trim().ToUpper();
        if (cleanedValue.Length != 3)
            throw new DomainException("Currency id must be exactly 3 characters long.", nameof(CurrencyId));

        return new CurrencyId(cleanedValue);
    }

    public override string ToString() { return Value; }
}
