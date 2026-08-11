using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public record Isin : ValueObject {
    public string Value { get; }

    private Isin(string value) {
        Value = value;
    }

    public static ErrorOr<Isin> Create(string value) {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorInfo.Unprocessable("Invalid ISIN.");

        var cleanedValue = value.Trim().ToUpper();
        if (cleanedValue.Length != 12)
            return ErrorInfo.Unprocessable("ISIN must be exactly 12 characters long.");

        return new Isin(cleanedValue);
    }

    public override string ToString() { return Value; }
}
