using Microsoft.EntityFrameworkCore;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using PAS.Persistence;

namespace PAS.Assets.Persistence;

public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContextBase(options, SchemaName) {
    public const string SchemaName = "Asset";

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Fund> Funds { get; set; }
    public DbSet<FundNav> FundNavs { get; set; }
}
