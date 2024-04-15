using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.AspNetCore;
using IntegrationMocks.Modules.Postgres;
using IntegrationMocks.Modules.Sql;
using IntegrationMocks.Sample.Locations.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public class UsersHostService : HostService<UsersHostServiceContract>
{
    private readonly IPort _webApiPort;
    private readonly IInfrastructureService<SqlServiceContract> _postgres;
    private readonly IInfrastructureService<LocationsMockContract> _locationsMock;

    public UsersHostService(
        IPortManager portManager,
        IInfrastructureService<SqlServiceContract> postgres,
        IInfrastructureService<LocationsMockContract> locationsMock)
    {
        _webApiPort = portManager.TakePort();
        _postgres = postgres;
        _locationsMock = locationsMock;
        Contract = new UsersHostServiceContract(_webApiPort.Number);
    }

    public override UsersHostServiceContract Contract { get; }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Kestrel:EndPoints:Http:Url"]
                        = $"http://localhost:{_webApiPort.Number}",
                    ["Persistence:ConnectionString"]
                        = _postgres.CreatePostgresConnectionString("users"),
                    ["Locations:BaseAddress"]
                        = $"http://localhost:{_locationsMock.Contract.WebApiPort}"
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
