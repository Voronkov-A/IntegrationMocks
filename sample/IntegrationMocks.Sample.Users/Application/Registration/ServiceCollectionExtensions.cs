using IntegrationMocks.Sample.Users.Application.Common;

namespace IntegrationMocks.Sample.Users.Application.Registration;

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
