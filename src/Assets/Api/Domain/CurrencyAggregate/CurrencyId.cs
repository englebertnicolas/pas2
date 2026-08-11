using PAS.Domain;

namespace PAS.Assets.Domain.CurrencyAggregate;

public readonly record struct CurrencyId : IStronglyTypedId<CurrencyId, string> {
    public string Value { get; }

    private CurrencyId(string value) => Value = value;

    public static ErrorOr<CurrencyId> From(string value) {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorInfo.Unprocessable("Invalid currency code.");

        var cleanedValue = value.Trim().ToUpper();
        if (cleanedValue.Length != 3)
            return ErrorInfo.Unprocessable("Currency ID must be exactly 3 characters long.");

        return new CurrencyId(cleanedValue);
    }

    public static CurrencyId Hydrate(string value) => new(value);

    public override string ToString() => Value;
}
