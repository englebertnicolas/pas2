using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate.Events;

public record FundClosedDomainEvent(
    Guid Id
) : IDomainEvent;
