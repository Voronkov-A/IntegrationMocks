using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationMocks.Modules.AspNetCore;

internal class DelegateControllerRegistrar : IControllerRegistrar
{
    public DelegateControllerRegistrar(Type controllerType, Action<IServiceCollection> register)
    {
        ControllerType = controllerType;
        _register = register;
    }

    private readonly Action<IServiceCollection> _register;

    public Type ControllerType { get; }

    public void Register(IServiceCollection services)
    {
        _register(services);
    }
}
