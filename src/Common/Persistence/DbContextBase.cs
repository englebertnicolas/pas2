using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace PAS.Persistence;

public abstract class DbContextBase(DbContextOptions options, string schemaName) : DbContext(options) {

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasDefaultSchema(schemaName);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // Est-ce vraiment nécessaire ?
        // (sachant que les tables Wolverine ne sont pas traitées par les migrations EF).
        builder.MapWolverineEnvelopeStorage();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Configure the migrations history table to be in the right schema
        optionsBuilder.UseSqlServer(x =>
            x.MigrationsHistoryTable("__EFMigrationsHistory", schemaName)
        );

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }
}
