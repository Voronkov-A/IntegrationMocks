using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Library.Sql;
using IntegrationMocks.Sample.Locations.Mocks;
using IntegrationMocks.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public class UsersHostService : HostService<DefaultHostServiceContract>
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
        Contract = new DefaultHostServiceContract(_webApiPort.Number);
    }

    public override DefaultHostServiceContract Contract { get; }

    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string?>()
            {
                ["Kestrel:EndPoints:Http:Url"] = UriUtils.HttpLocalhost(_webApiPort.Number).ToString(),
                ["Persistence:ConnectionString"] = _postgres.CreatePostgresConnectionString("users"),
                ["Locations:BaseAddress"] = UriUtils.HttpLocalhost(_locationsMock.Contract.WebApiPort).ToString()
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
