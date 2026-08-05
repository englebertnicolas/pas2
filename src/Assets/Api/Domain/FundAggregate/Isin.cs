using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public record Isin : ValueObject {
    public string Value { get; }

    private Isin(string value) {
        Value = value;
    }

    public static Isin Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Invalid ISIN.", nameof(Isin));

        var cleanedValue = value.Trim().ToUpper();
        if (cleanedValue.Length != 12)
            throw new DomainException("ISIN must be exactly 12 characters long.", nameof(Isin));

        return new Isin(cleanedValue);
    }

    public override string ToString() { return Value; }
}
