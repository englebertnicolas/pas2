using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate.Events;

public record FundNavChangedDomainEvent(
    long FundId,
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double? NavOldValue,
    double NavNewValue
) : IDomainEvent;
