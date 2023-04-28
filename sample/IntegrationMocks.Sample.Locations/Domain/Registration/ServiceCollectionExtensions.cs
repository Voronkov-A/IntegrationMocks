namespace IntegrationMocks.Sample.Locations.Domain.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services.AddSingleton<ILocationFactory, LocationFactory>();
    }
}
