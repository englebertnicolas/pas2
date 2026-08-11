using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public record FundNav : ValueObject {
    public DateTime Date { get; private set; }
    public double Value { get; private set; }

    private FundNav() {
        // For EF hydration
    }

    private FundNav(DateTime date, double value) {
        Date = date;
        Value = value;
    }

    public static ErrorOr<FundNav> Create(DateTime date, double value) {
        if (date < new DateTime(1900, 1, 1))
            return ErrorInfo.Unprocessable("Invalid NAV date.");

        if (date > DateTime.Now)
            return ErrorInfo.Unprocessable("Fund NAV date cannot be in the future.");

        if (value <= 0)
            return ErrorInfo.Unprocessable("Fund NAV must be greater than zero.");

        return new FundNav(date, value);
    }
}
