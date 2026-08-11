using PAS.Assets.Contracts;
using PAS.Assets.Domain.FundAggregate.Events;
using Wolverine;

namespace PAS.Assets.Features.Funds;

public class HandleFundClosed : IWolverineHandler {

    public static ValueTask HandleAsync(FundClosedDomainEvent domainEvent, IMessageBus bus) {
        // Raise an integration event to the message broker to notify other services about the change
        return bus.PublishAsync(new FundClosedIntegrationEvent(
           domainEvent.Id
       ));
    }
}
