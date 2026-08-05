using Microsoft.EntityFrameworkCore;
using PAS.Persistence;

namespace PAS.ActuarialEngine.Persistence;

public class ActuDbContext(DbContextOptions<ActuDbContext> options) : DbContextBase(options, SchemaName) {
    public const string SchemaName = "Actu";

    // Define DbSet properties for entities here
}
