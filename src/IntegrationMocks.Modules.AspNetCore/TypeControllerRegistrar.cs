using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationMocks.Modules.AspNetCore;

internal sealed class TypeControllerRegistrar : IControllerRegistrar
{
    public TypeControllerRegistrar(Type controllerType)
    {
        ControllerType = controllerType;
    }

    public Type ControllerType { get; }

    public void Register(IServiceCollection services)
    {
        services.AddScoped(ControllerType);
    }
}
