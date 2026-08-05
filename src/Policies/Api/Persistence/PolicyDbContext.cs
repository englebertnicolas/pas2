using Microsoft.EntityFrameworkCore;
using PAS.Persistence;

namespace PAS.Policies.Persistence;

public class PolicyDbContext(DbContextOptions<PolicyDbContext> options) : DbContextBase(options, SchemaName) {
    public const string SchemaName = "Policy";

    // Define DbSet properties for entities here
}
