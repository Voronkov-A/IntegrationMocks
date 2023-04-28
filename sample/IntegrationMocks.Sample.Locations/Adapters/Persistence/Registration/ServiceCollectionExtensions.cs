using IntegrationMocks.Sample.Locations.Adapters.Persistence.Common;
using IntegrationMocks.Sample.Locations.Application.Common;
using IntegrationMocks.Sample.Locations.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Locations.Adapters.Persistence.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        return services
            .AddDbContext<PersistenceContext>(
                x => x.UseNpgsql(options.ConnectionString),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton)
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<IUnitOfWork, DbContextUnitOfWork<PersistenceContext>>();
    }

    public static IServiceCollection AddPersistenceMigrations(this IServiceCollection services)
    {
        return services.AddHostedService<PersistenceMigrator>();
    }
}
