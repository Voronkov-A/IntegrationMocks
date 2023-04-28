using IntegrationMocks.Sample.Users.Adapters.Persistence.Configurations;
using IntegrationMocks.Sample.Users.Adapters.Persistence.Converters;
using IntegrationMocks.Sample.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence;

public class PersistenceContext : DbContext
{
    public PersistenceContext(DbContextOptions<PersistenceContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Location>().HaveConversion<LocationValueConverter>();
    }
}
