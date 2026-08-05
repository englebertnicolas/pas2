using PAS.ActuarialEngine.Persistence;
using PAS.Assets.Contracts;
using Wolverine;

namespace PAS.ActuarialEngine.Features.RetroactiveChanges;

public partial class EvaluateFundNavChange : IWolverineHandler {

    public static Task ConsumeAsync(FundNavChangedIntegrationEvent @event, ActuDbContext dbContext, ILogger<EvaluateFundNavChange> logger, CancellationToken ct) {
        LogEventReceived(logger, @event.FundId, @event.NavDate, @event.NavOldValue, @event.NavNewValue);

        // TODO: rechercher les polices concernées par le changement de NAV
        // et planifier un éventuel recalcul de la valorisation de ces polices.

        return Task.CompletedTask;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Received FundNavChangedIntegrationEvent for FundId: {FundId}, NavDate: {NavDate}, NavOldValue: {NavOldValue}, NavNewValue: {NavNewValue}")]
    public static partial void LogEventReceived(ILogger logger, long fundId, DateTime navDate, double? navOldValue, double navNewValue);
}
