using PAS.Assets.Contracts;
using PAS.Assets.Domain.FundAggregate.Events;
using Wolverine;

namespace PAS.Assets.Features.Funds;

public class HandleFundNavChanged : IWolverineHandler {

    public static ValueTask HandleAsync(FundNavChangedDomainEvent domainEvent, IMessageBus bus) {
        // Raise an integration event to the message broker to notify other services about the NAV update
        return bus.PublishAsync(new FundNavChangedIntegrationEvent(
           domainEvent.FundId,
           domainEvent.FundIsin,
           domainEvent.FundCurrency,
           domainEvent.NavDate,
           domainEvent.NavOldValue,
           domainEvent.NavNewValue
       ));
    }
}
