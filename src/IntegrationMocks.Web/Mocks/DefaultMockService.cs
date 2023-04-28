using IntegrationMocks.Core.Networking;
using IntegrationMocks.Web.Hosting;
using Microsoft.AspNetCore.Builder;

namespace IntegrationMocks.Web.Mocks;

public abstract class DefaultMockService<TContract> : WebApplicationService<TContract>
{
    private readonly List<IControllerRegistrar> _controllerRegistrars;

    protected DefaultMockService(IPortManager portManager)
    {
        WebApiPort = portManager.TakePort();
        _controllerRegistrars = new List<IControllerRegistrar>();
    }

    protected IPort WebApiPort { get; }

    protected void AddController(object controller)
    {
        _controllerRegistrars.Add(new ControllerRegistrar(controller));
    }

    protected override WebApplicationBuilder CreateWebApplicationBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["Kestrel:EndPoints:Http:Url"] = $"http://+:{WebApiPort.Number}";
        builder.Services.AddExplicitControllers(_controllerRegistrars);
        return builder;
    }

    protected override void Configure(WebApplication app)
    {
        app.UseRouting();
        app.MapControllers();
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        await base.DisposeAsync(disposing);

        if (disposing)
        {
            WebApiPort.Dispose();
        }
    }
}
