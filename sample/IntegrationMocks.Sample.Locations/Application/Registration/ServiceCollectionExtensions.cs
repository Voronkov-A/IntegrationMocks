using IntegrationMocks.Sample.Locations.Application.Common;

namespace IntegrationMocks.Sample.Locations.Application.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediatR(c => c
                .RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly)
                .AddOpenBehavior(typeof(UnitOfWorkPipelineBehavior<,>), ServiceLifetime.Scoped));
    }
}
