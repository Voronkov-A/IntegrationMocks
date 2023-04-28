using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Web.Mocks;

public interface IControllerRegistrar
{
    Type ControllerType { get; }

    void Register(IServiceCollection services);
}
