using System;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.AspNetCore;
using IntegrationMocks.Modules.Postgres;
using IntegrationMocks.Sample.Locations.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

internal sealed class UsersHostService : HostService<UsersHostServiceContract>
{
    private readonly IPort _webApiPort;
    private readonly IInfrastructureService<PostgresServiceContract> _postgres;
    private readonly IInfrastructureService<LocationsMockContract> _locationsMock;

    public UsersHostService(
        IPortManager portManager,
        IInfrastructureService<PostgresServiceContract> postgres,
        IInfrastructureService<LocationsMockContract> locationsMock)
    {
        _webApiPort = portManager.TakePort();
        _postgres = postgres;
        _locationsMock = locationsMock;
        Contract = new UsersHostServiceContract
        {
            WebApiUrl = new Uri($"http://localhost:{_webApiPort.Number}")
        };
    }

    public override UsersHostServiceContract Contract { get; }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Kestrel:EndPoints:Http:Url"] = Contract.WebApiUrl.ToString(),
                    ["Persistence:ConnectionString"] = _postgres.CreatePostgresConnectionString("users"),
                    ["Locations:BaseAddress"] = _locationsMock.Contract.WebApiUrl.ToString()
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
