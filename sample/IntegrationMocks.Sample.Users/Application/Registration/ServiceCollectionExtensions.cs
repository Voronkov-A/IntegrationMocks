using IntegrationMocks.Sample.Users.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Sample.Users.Application.Registration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediatR(c => c
                .RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly)
                .AddOpenBehavior(typeof(UnitOfWorkPipelineBehavior<,>), ServiceLifetime.Scoped));
    }
}
