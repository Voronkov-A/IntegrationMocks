using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Sample.Locations.Domain.Registration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services.AddSingleton<ILocationFactory, LocationFactory>();
    }
}
