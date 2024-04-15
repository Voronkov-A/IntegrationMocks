using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationMocks.Modules.AspNetCore;

internal interface IControllerRegistrar
{
    Type ControllerType { get; }

    void Register(IServiceCollection services);
}
