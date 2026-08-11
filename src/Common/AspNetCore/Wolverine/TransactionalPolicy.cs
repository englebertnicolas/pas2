using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Core.Reflection;
using Wolverine.Attributes;
using Wolverine.Configuration;
using Wolverine.Runtime.Handlers;

namespace PAS.AspNetCore.Wolverine;

internal class TransactionalPolicy : IHandlerPolicy {

    public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container) {
        // Add TransactionalAttribute to the command handlers if it doesn't already have one
        var commandChains = chains.Where(chain => chain.MessageType.Name.EndsWith("Command"));
        foreach (var chain in commandChains) {
            new TransactionalAttribute().Modify(chain, rules, container);
        }

        // Check that TransactionalAttribute is NOT present on query handlers
        var queryChains = chains.Where(chain => chain.MessageType.Name.EndsWith("Query"));
        foreach (var chain in queryChains) {
            var invalidHandler = chain
                .Handlers
                .FirstOrDefault(h => h.Method.HasAttribute<TransactionalAttribute>() || h.HandlerType.HasAttribute<TransactionalAttribute>());

            if (invalidHandler != null)
                throw new InvalidOperationException($"Query handler '{invalidHandler.HandlerType.Name}.{invalidHandler.Method.Name}' should not have a TransactionalAttribute.");
        }
    }
}
