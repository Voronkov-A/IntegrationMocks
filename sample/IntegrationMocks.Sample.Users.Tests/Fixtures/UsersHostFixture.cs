using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Postgres;
using IntegrationMocks.Modules.Sql;
using IntegrationMocks.Sample.Locations.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public sealed class UsersHostFixture : IAsyncLifetime, IDisposable
{
    public UsersHostFixture()
    {
        var portManager = PortManager.Default;
        Postgres = new DockerPostgresService();
        Locations = new LocationsMockService(portManager);
        Users = new UsersHostService(portManager, Postgres, Locations);
    }

    public IInfrastructureService<SqlServiceContract> Postgres { get; }

    public IInfrastructureService<LocationsMockContract> Locations { get; }

    public IInfrastructureService<UsersHostServiceContract> Users { get; }

    public async Task InitializeAsync()
    {
        await Postgres.InitializeAsync();
        await Locations.InitializeAsync();
        await Users.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await Users.DisposeAsync();
        await Locations.DisposeAsync();
        await Postgres.DisposeAsync();
    }

    public void Dispose()
    {
        Users.Dispose();
        Locations.Dispose();
        Postgres.Dispose();
    }
}
