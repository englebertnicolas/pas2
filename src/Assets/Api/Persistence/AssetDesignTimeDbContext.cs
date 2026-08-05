using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PAS.Assets.Persistence;

/// <summary>
/// Design-time factory for WorkflowStore.
/// Used by EF Core tools (migrations, updates) when no DI container is available.
/// </summary>
internal class AssetDesignTimeDbContext : IDesignTimeDbContextFactory<AssetDbContext> {

    public AssetDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<AssetDbContext>();
        optionsBuilder.UseSqlServer("Server=.\\dbloc19;Database=PAS;Trusted_Connection=True;Encrypt=False");
        return new AssetDbContext(optionsBuilder.Options);
    }
}
