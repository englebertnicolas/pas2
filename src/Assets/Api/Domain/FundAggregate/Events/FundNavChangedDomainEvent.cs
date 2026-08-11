using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate.Events;

public record FundNavChangedDomainEvent(
    Guid FundId,
    DateTime Date,
    double? OldValue,
    double NewValue
) : IDomainEvent;
