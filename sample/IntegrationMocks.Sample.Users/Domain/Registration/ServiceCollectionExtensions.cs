namespace IntegrationMocks.Sample.Users.Domain.Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services.AddSingleton<IUserFactory, UserFactory>();
    }
}
