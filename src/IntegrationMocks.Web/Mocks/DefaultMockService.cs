using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationMocks.Web.Mocks;

public abstract class DefaultMockService<TContract> : WebApplicationService<TContract>
{
    private readonly List<IControllerRegistrar> _controllerRegistrars;

    protected DefaultMockService(IPortManager portManager, Range<int> portRange)
    {
        WebApiPort = portManager.TakePort();
        _controllerRegistrars = new List<IControllerRegistrar>();
    }

    protected DefaultMockService(IPortManager portManager)
        : this(portManager, PortRange.Default)
    {
    }

    protected IPort WebApiPort { get; }

    protected void AddController(object controller)
    {
        _controllerRegistrars.Add(new ControllerRegistrar(controller));
    }

    protected override WebApplicationBuilder CreateWebApplicationBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel(opt => opt.ListenAnyIP(WebApiPort.Number));
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
