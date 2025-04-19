using System;

namespace IntegrationMocks.Core.Environments;

public static class ServiceBinding
{
    public static IServiceBinding<TContract> Create<TContract>(
        string environmentVariableName,
        string environmentVariableValue,
        Func<IInfrastructureService<TContract>> factory)
    {
        return new EnvironmentVariableEqualsServiceBinding<TContract>(
            environmentVariableName,
            environmentVariableValue,
            factory);
    }

    public static IServiceBinding<TContract> Create<TContract>(
        string environmentName,
        Func<string, IInfrastructureService<TContract>> factory)
    {
        return new EnvironmentVariableExistsServiceBinding<TContract>(environmentName, factory);
    }

    public static IServiceBinding<TContract> Create<TContract>(Func<IInfrastructureService<TContract>> factory)
    {
        return new DefaultServiceBinding<TContract>(factory);
    }
}
