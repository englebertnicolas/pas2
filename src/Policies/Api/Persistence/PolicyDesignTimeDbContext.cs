using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PAS.Policies.Persistence;

/// <summary>
/// Design-time factory for WorkflowStore.
/// Used by EF Core tools (migrations, updates) when no DI container is available.
/// </summary>
internal class PolicyDesignTimeDbContext : IDesignTimeDbContextFactory<PolicyDbContext> {

    public PolicyDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<PolicyDbContext>();
        optionsBuilder.UseSqlServer("Server=.\\dbloc19;Database=PAS;Trusted_Connection=True;Encrypt=False");
        return new PolicyDbContext(optionsBuilder.Options);
    }
}
