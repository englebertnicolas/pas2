namespace PAS.Assets.Contracts;

public record FundClosedIntegrationEvent(
    Guid Id
) : IIntegrationEvent;
