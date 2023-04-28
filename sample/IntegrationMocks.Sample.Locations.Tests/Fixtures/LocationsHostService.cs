using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Library.Sql;
using IntegrationMocks.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

public class LocationsHostService : HostService<DefaultHostServiceContract>
{
    private readonly IPort _webApiPort;
    private readonly IInfrastructureService<SqlServiceContract> _postgres;

    public LocationsHostService(IPortManager portManager, IInfrastructureService<SqlServiceContract> postgres)
    {
        _webApiPort = portManager.TakePort();
        _postgres = postgres;
        Contract = new DefaultHostServiceContract(_webApiPort.Number);
    }

    public override DefaultHostServiceContract Contract { get; }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["Kestrel:EndPoints:Http:Url"] = UriUtils.HttpLocalhost(_webApiPort.Number).ToString(),
                ["Persistence:ConnectionString"] = _postgres.CreatePostgresConnectionString("locations")
            }))
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        await base.DisposeAsync(disposing);

        if (disposing)
        {
            _webApiPort.Dispose();
        }
    }
}
