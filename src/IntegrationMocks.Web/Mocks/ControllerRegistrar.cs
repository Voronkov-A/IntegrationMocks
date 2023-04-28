using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Web.Mocks;

public class ControllerRegistrar : IControllerRegistrar
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
