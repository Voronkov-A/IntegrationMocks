using System;

namespace IntegrationMocks.Core.Environments;

internal sealed class EnvironmentVariableExistsServiceBinding<TContract> : IServiceBinding<TContract>
{
    private readonly string _environmentVariableName;
    private readonly Func<string, IInfrastructureService<TContract>> _factory;

    public EnvironmentVariableExistsServiceBinding(
        string environmentVariableName,
        Func<string, IInfrastructureService<TContract>> factory)
    {
        _environmentVariableName = environmentVariableName;
        _factory = factory;
    }

    public IInfrastructureService<TContract>? GetOrDefault()
    {
        var environmentVariableValue = Environment.GetEnvironmentVariable(_environmentVariableName);
        return environmentVariableValue == null ? null : _factory(environmentVariableValue);
    }
}
