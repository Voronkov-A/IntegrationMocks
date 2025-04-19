using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Postgres;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

public sealed class LocationsHostFixture : IAsyncLifetime, IDisposable
{
    public LocationsHostFixture()
    {
        var portManager = PortManager.Default;
        Postgres = new DockerPostgresService();
        Locations = new LocationsHostService(portManager, Postgres);
    }

    internal IInfrastructureService<PostgresServiceContract> Postgres { get; }

    internal IInfrastructureService<LocationsHostServiceContract> Locations { get; }

    public async Task InitializeAsync()
    {
        await Postgres.InitializeAsync();
        await Locations.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await Locations.DisposeAsync();
        await Postgres.DisposeAsync();
    }

    public void Dispose()
    {
        Locations.Dispose();
        Postgres.Dispose();
    }
}
