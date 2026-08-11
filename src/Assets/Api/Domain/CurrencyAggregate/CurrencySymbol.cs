using PAS.Domain;

namespace PAS.Assets.Domain.CurrencyAggregate;

public record CurrencySymbol : ValueObject {
    public string Value { get; }

    private CurrencySymbol(string value) {
        Value = value;
    }

    public static ErrorOr<CurrencySymbol?> CreateOrNull(string? value) {
        if (value == null) return (CurrencySymbol?)null;
        return Create(value)
            .Bind(cs => ErrorOr<CurrencySymbol?>.Success(cs));
    }

    public static ErrorOr<CurrencySymbol> Create(string value) {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorInfo.Unprocessable("Invalid currency symbol.");

        var cleanedValue = value.Trim();
        if (cleanedValue.Length < 1 || cleanedValue.Length > 3)
            return ErrorInfo.Unprocessable("Currency symbol must be between 1 and 3 characters long.");

        return new CurrencySymbol(cleanedValue);
    }

    public override string ToString() { return Value; }
}
