namespace PAS.Assets.Contracts;

public record FundNavChangedIntegrationEvent(
    long FundId,
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double? NavOldValue,
    double NavNewValue
) : IIntegrationEvent;
