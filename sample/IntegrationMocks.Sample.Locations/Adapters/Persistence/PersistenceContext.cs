using IntegrationMocks.Sample.Locations.Adapters.Persistence.Configurations;
using IntegrationMocks.Sample.Locations.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Locations.Adapters.Persistence;

public class PersistenceContext : DbContext
{
    public PersistenceContext(DbContextOptions<PersistenceContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new LocationConfiguration());
    }
}
