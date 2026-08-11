using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace PAS.AspNetCore.Wolverine;

public static class DbContextOutboxExtensions {

    /// <summary>
    /// Executes an action within an explicit database transaction, persists entity changes, 
    /// and flushes the Wolverine outbox messages.
    /// </summary>
    /// <remarks>
    /// This method is designed to be used exclusively OUTSIDE of the native Wolverine
    /// execution pipeline. Do not manually call SaveChanges inside the task.
    /// </remarks>
    public static async Task ExecuteInTransactionAsync<TDbContext>(
        this IDbContextOutbox<TDbContext> outbox,
        Func<TDbContext, Task> action
    ) where TDbContext : DbContext {

        await using var transaction = await outbox.DbContext.Database.BeginTransactionAsync();
        await action(outbox.DbContext);
        await outbox.SaveChangesAndFlushMessagesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Executes an action within an explicit database transaction, persists entity changes, 
    /// and flushes the Wolverine outbox messages.
    /// </summary>
    /// <remarks>
    /// This method is designed to be used exclusively OUTSIDE of the native Wolverine
    /// execution pipeline. Do not manually call SaveChanges inside the task.
    /// </remarks>
    public static Task ExecuteInTransactionAsync<TDbContext>(
        this IDbContextOutbox<TDbContext> outbox,
        Action<TDbContext> action
    ) where TDbContext : DbContext {
        return outbox.ExecuteInTransactionAsync(dbContext => {
            action(dbContext);
            return Task.CompletedTask;
        });
    }
}
