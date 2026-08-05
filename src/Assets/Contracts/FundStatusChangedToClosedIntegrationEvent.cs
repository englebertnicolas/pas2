namespace PAS.Assets.Contracts;

public record FundStatusChangedToClosedIntegrationEvent(
    long Id
) : IIntegrationEvent;
