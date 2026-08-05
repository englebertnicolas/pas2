using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PAS.AspNetCore.Wolverine;
using PAS.Domain;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.RabbitMQ;
using Wolverine.SqlServer;

namespace PAS.AspNetCore.Configuration;

public static class WolverineExtensions {

    public static IServiceCollection AddDefaultWolverine(this IServiceCollection services, string dbCnc, string dbSchemaName,
        string rabbitMqCnc, bool autoProvisionRabbitMq = false, Assembly[]? discoveryAssemblies = null,
        Action<WolverineOptions>? configure = null) {

        return services.AddWolverine(options => {
            options.CodeGeneration.TypeLoadMode = JasperFx.CodeGeneration.TypeLoadMode.Auto;

            // Forces the API to act independently rather than forming a cluster. This prevents startup errors when
            // multiple services boot simultaneously in .NET Aspire while sharing the same database.
            options.Durability.Mode = DurabilityMode.Solo;

            // Only discover message handlers that implement the IWolverineHandler interface.
            options.Discovery.DisableConventionalDiscovery();
            options.Discovery.CustomizeHandlerDiscovery(config => {
                config.Includes.Implements<IWolverineHandler>();
            });

            // Scans external assemblies to discover message handlers, policies and validators.
            if (discoveryAssemblies != null)
                foreach (var assembly in discoveryAssemblies)
                    options.Discovery.IncludeAssembly(assembly);

            // Configures Microsoft SQL Server as both the backing store for Wolverine's database-backed message queues.
            // and the transactional inbox/outbox storage
            options.UseSqlServerPersistenceAndTransport(dbCnc, dbSchemaName);

            // Integrates Entity Framework Core into Wolverine's execution pipeline,
            // allowing EF Core DbContexts to coordinate with Wolverine transactions.
            options.UseEntityFrameworkCoreTransactions();

            // Automatically injects transactional middleware (and triggers SaveChangesAsync)
            // for commands.
            options.Policies.Add<HandlerTransactionPolicy>();

            // Instructs Wolverine to automatically intercept, extract, and publish domain events
            // from tracked EF entities (that inherit from 'Entity') before committing changes.
            options.PublishDomainEventsFromEntityFrameworkCore<Entity>(x => x.DomainEvents);

            // Plugs FluentValidation into the execution pipeline to automatically validate
            // incoming messages before they reach their respective handlers.
            options.UseFluentValidation();

            // Configures RabbitMQ as an external message broker transport layer.
            var rabbitMqOptions = options.UseRabbitMq(new Uri(rabbitMqCnc))
                // Automatically maps messages to RabbitMQ exchanges/queues based on naming conventions
                // (rather than manual registration).
                .UseConventionalRouting(routingConvention => {
                    // Restricts conventional routing so that only messages that implement
                    // IIntegrationEvent are routed out to RabbitMQ.
                    routingConvention.IncludeTypes(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

                    // Endpoints discovered at runtime via conventional routing escape the global 
                    // UseDurableOutboxOnAllSendingEndpoints() policy. This explicitly forces every dynamically 
                    // discovered RabbitMQ endpoint to use the Transactional Outbox, preventing race conditions 
                    // where messages are sent before the SQL Server transaction commits.
                    routingConvention.ConfigureSending((endpoint, _) => endpoint.UseDurableOutbox());
                });

            if (autoProvisionRabbitMq) {
                // Forces Wolverine to automatically create missing RabbitMQ queues, exchanges, and bindings at startup.
                rabbitMqOptions.AutoProvision();
            }

            /*
            // Ensures guaranteed delivery by enforcing a Transactional Outbox pattern
            // for all outgoing messages (persisting them in SQL Server before sending).
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();
            */

            // Ensures resilient processing by enforcing a Transactional Inbox pattern
            // for all incoming listeners (persisting incoming messages to handle failures/retries safely).
            options.Policies.UseDurableInboxOnAllListeners();

            // Automatically executes Wolverine database schema migrations at application startup.
            options.UseEntityFrameworkCoreWolverineManagedMigrations();

            // Configures retry policy.
            options.Policies.OnException(ex => ex is TimeoutException || ex is HttpRequestException)
                .RetryWithCooldown(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5))
                .Then.ScheduleRetry(TimeSpan.FromHours(1), TimeSpan.FromHours(23));

            // Configures the maximum number of threads available at one time for all integration event listeners.
            options.Policies.AllListeners(queue => queue.MaximumParallelMessages(5));

            configure?.Invoke(options);
        });
    }
}
