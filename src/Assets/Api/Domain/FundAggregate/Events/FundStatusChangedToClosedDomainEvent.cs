using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate.Events;

public record FundStatusChangedToClosedDomainEvent(
    long Id
) : IDomainEvent;
