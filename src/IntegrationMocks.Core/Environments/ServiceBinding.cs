namespace IntegrationMocks.Core.Environments;

public class ServiceBinding<TContract>
{
    public ServiceBinding(string environmentName, Func<IInfrastructureService<TContract>> factory)
    {
        EnvironmentName = environmentName;
        Factory = factory;
    }

    public ServiceBinding(Func<IInfrastructureService<TContract>> factory)
    {
        EnvironmentName = null;
        Factory = factory;
    }

    public string? EnvironmentName { get; }

    public Func<IInfrastructureService<TContract>> Factory { get; }
}

public static class ServiceBinding
{
    public static ServiceBinding<TContract> Create<TContract>(
        string environmentName,
        Func<IInfrastructureService<TContract>> factory)
    {
        return new ServiceBinding<TContract>(environmentName, factory);
    }

    public static ServiceBinding<TContract> Create<TContract>(Func<IInfrastructureService<TContract>> factory)
    {
        return new ServiceBinding<TContract>(factory);
    }
}
