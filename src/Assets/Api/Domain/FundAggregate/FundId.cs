using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public readonly record struct FundId : IStronglyTypedId<FundId, Guid> {
    public Guid Value { get; }

    private FundId(Guid value) => Value = value;

    public static FundId New() => new(Guid.NewGuid());

    public static ErrorOr<FundId> From(Guid value) {
        if (value == Guid.Empty)
            return ErrorInfo.Unprocessable("Invalid fund ID.");

        return new FundId(value);
    }

    public static ErrorOr<FundId> FromOrNew(Guid? value) => value.HasValue ? From(value.Value) : New();

    public static FundId Hydrate(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
