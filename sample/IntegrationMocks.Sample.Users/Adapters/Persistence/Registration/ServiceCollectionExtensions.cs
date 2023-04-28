using IntegrationMocks.Sample.Users.Adapters.Persistence.Common;
using IntegrationMocks.Sample.Users.Application.Common;
using IntegrationMocks.Sample.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        return services
            .AddDbContext<PersistenceContext>(
                x => x.UseNpgsql(options.ConnectionString),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton)
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUnitOfWork, DbContextUnitOfWork<PersistenceContext>>();
    }

    public static IServiceCollection AddPersistenceMigrations(this IServiceCollection services)
    {
        return services.AddHostedService<PersistenceMigrator>();
    }
}
