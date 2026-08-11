namespace PAS.Assets.Contracts;

public record FundNavChangedIntegrationEvent(
    Guid FundId,
    DateTime Date,
    double? OldValue,
    double NewValue
) : IIntegrationEvent;
