using IntegrationMocks.Sample.Users.Domain;

namespace IntegrationMocks.Sample.Users.Adapters.Locations.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocations(this IServiceCollection services, LocationsOptions options)
    {
        return services
            .AddSingleton(options)
            .AddScoped<ILocationRepository, HttpLocationRepository>();
    }
}
