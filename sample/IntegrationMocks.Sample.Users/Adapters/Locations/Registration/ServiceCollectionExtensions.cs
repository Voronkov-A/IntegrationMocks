using IntegrationMocks.Sample.Users.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Sample.Users.Adapters.Locations.Registration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocations(
        this IServiceCollection services,
        LocationsOptions options)
    {
        return services
            .AddSingleton(options)
            .AddScoped<ILocationRepository, HttpLocationRepository>();
    }
}
