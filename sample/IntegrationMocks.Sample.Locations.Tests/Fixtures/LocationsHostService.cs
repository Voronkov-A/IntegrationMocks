using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.AspNetCore;
using IntegrationMocks.Modules.Postgres;
using IntegrationMocks.Modules.Sql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

public class LocationsHostService : HostService<LocationsHostServiceContract>
{
    private readonly IPort _webApiPort;
    private readonly IInfrastructureService<SqlServiceContract> _postgres;

    public LocationsHostService(
        IPortManager portManager,
        IInfrastructureService<SqlServiceContract> postgres)
    {
        _webApiPort = portManager.TakePort();
        _postgres = postgres;
        Contract = new LocationsHostServiceContract(_webApiPort.Number);
    }

    public override LocationsHostServiceContract Contract { get; }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Kestrel:EndPoints:Http:Url"]
                        = $"http://localhost:{_webApiPort.Number}",
                    ["Persistence:ConnectionString"]
                        = _postgres.CreatePostgresConnectionString("locations")
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
