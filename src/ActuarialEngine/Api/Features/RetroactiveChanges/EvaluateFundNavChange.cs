using PAS.ActuarialEngine.Persistence;
using PAS.Assets.Contracts;
using Wolverine;

namespace PAS.ActuarialEngine.Features.RetroactiveChanges;

public partial class EvaluateFundNavChange : IWolverineHandler {

    public static Task ConsumeAsync(FundNavChangedIntegrationEvent @event, ActuDbContext dbContext, ILogger<EvaluateFundNavChange> logger, CancellationToken ct) {
        LogEventReceived(logger, @event.FundId, @event.Date, @event.OldValue, @event.NewValue);

        // TODO: rechercher les polices concernées par le changement de NAV
        // et planifier un éventuel recalcul de la valorisation de ces polices.

        return Task.CompletedTask;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Received FundNavChangedIntegrationEvent for FundId: {FundId}, Date: {Date}, OldValue: {OldValue}, NewValue: {NewValue}")]
    public static partial void LogEventReceived(ILogger logger, Guid fundId, DateTime date, double? oldValue, double newValue);
}
