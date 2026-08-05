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

    public static FundNav Create(DateTime date, double value) {
        if (date < new DateTime(1900, 1, 1))
            throw new DomainException("Invalid NAV date.", nameof(Date));
        if (date > DateTime.Now)
            throw new DomainException("Fund NAV date cannot be in the future.", nameof(Date));
        if (value <= 0)
            throw new DomainException("Fund NAV must be greater than zero.", nameof(Value));

        return new FundNav(date, value);
    }
}
