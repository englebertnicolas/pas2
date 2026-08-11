using PAS.Assets.Contracts;
using PAS.Assets.Domain.FundAggregate.Events;
using Wolverine;

namespace PAS.Assets.Features.Funds;

public class HandleFundNavChanged : IWolverineHandler {

    public ValueTask HandleAsync(FundNavChangedDomainEvent domainEvent, IMessageBus bus) {
        // Raise an integration event to the message broker to notify other services about the NAV update
        return bus.PublishAsync(new FundNavChangedIntegrationEvent(
           domainEvent.FundId,
           domainEvent.Date,
           domainEvent.OldValue,
           domainEvent.NewValue
       ));
    }
}
