using System;

namespace IntegrationMocks.Core.Environments;

internal sealed class EnvironmentVariableEqualsServiceBinding<TContract> : IServiceBinding<TContract>
{
    private readonly string _environmentVariableName;
    private readonly string _environmentVariableValue;
    private readonly Func<IInfrastructureService<TContract>> _factory;

    public EnvironmentVariableEqualsServiceBinding(
        string environmentVariableName,
        string environmentVariableValue,
        Func<IInfrastructureService<TContract>> factory)
    {
        _environmentVariableName = environmentVariableName;
        _environmentVariableValue = environmentVariableValue;
        _factory = factory;
    }

    public IInfrastructureService<TContract>? GetOrDefault()
    {
        return Environment.GetEnvironmentVariable(_environmentVariableName) == _environmentVariableValue
            ? _factory()
            : null;
    }
}
