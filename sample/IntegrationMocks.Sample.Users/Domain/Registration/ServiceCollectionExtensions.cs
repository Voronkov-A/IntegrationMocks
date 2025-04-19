using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Sample.Users.Domain.Registration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services.AddSingleton<IUserFactory, UserFactory>();
    }
}
