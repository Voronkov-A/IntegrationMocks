using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationMocks.Modules.AspNetCore;

internal class ControllerRegistrar : IControllerRegistrar
{
    private readonly object _controller;

    public ControllerRegistrar(object controller)
    {
        _controller = controller;
    }

    public Type ControllerType => _controller.GetType();

    public void Register(IServiceCollection services)
    {
        services.AddSingleton(ControllerType, _controller);
    }
}
