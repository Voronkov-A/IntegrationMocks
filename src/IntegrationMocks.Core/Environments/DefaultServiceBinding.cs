using System;

namespace IntegrationMocks.Core.Environments;

internal sealed class DefaultServiceBinding<TContract> : IServiceBinding<TContract>
{
    private readonly Func<IInfrastructureService<TContract>> _factory;

    public DefaultServiceBinding(Func<IInfrastructureService<TContract>> factory)
    {
        _factory = factory;
    }

    public IInfrastructureService<TContract> GetOrDefault()
    {
        return _factory();
    }
}
