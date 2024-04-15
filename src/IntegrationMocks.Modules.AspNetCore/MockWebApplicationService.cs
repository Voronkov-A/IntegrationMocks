using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.AspNetCore;

public abstract class MockWebApplicationService<TInterface> : WebApplicationService<TInterface>
{
    private readonly List<IControllerRegistrar> _controllerRegistrars;

    protected MockWebApplicationService(IPortManager portManager, Range<int> portRange)
    {
        WebApiPort = portManager.TakePort(portRange);
        _controllerRegistrars = new List<IControllerRegistrar>();
    }

    protected MockWebApplicationService(IPortManager portManager)
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
        var mvcBuilder = builder.Services.AddExplicitControllers(_controllerRegistrars);
        ConfigureMvc(mvcBuilder);
        return builder;
    }

    protected override void Configure(WebApplication app)
    {
        app.UseRouting();
        app.MapControllers();
    }

    protected virtual void ConfigureMvc(IMvcBuilder builder)
    {
        // pass
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        await base.DisposeAsync(disposing);

        if (disposing)
        {
            WebApiPort?.Dispose();
        }
    }
}
