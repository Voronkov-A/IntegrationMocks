using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationMocks.Modules.AspNetCore;

public interface IControllerRegistrar
{
    Type ControllerType { get; }

    void Register(IServiceCollection services);
}
