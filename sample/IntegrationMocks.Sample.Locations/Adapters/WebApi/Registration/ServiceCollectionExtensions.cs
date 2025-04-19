using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Sample.Locations.Adapters.WebApi.Registration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        return services;
    }
}
