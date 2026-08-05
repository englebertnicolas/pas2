using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PAS.ActuarialEngine.Persistence;

/// <summary>
/// Design-time factory for WorkflowStore.
/// Used by EF Core tools (migrations, updates) when no DI container is available.
/// </summary>
internal class ActuDesignTimeDbContext : IDesignTimeDbContextFactory<ActuDbContext> {

    public ActuDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<ActuDbContext>();
        optionsBuilder.UseSqlServer("Server=.\\dbloc19;Database=PAS;Trusted_Connection=True;Encrypt=False");
        return new ActuDbContext(optionsBuilder.Options);
    }
}
