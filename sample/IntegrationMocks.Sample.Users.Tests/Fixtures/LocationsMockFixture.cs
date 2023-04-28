using IntegrationMocks.Core;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Sample.Locations.Mocks;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public sealed class LocationsMockFixture : IAsyncLifetime, IDisposable
{
    public LocationsMockFixture()
    {
        Locations = new LocationsMockService(PortManager.Default);
    }

    public IInfrastructureService<LocationsMockContract> Locations { get; }

    public async Task InitializeAsync()
    {
        await Locations.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await Locations.DisposeAsync();
    }

    public void Dispose()
    {
        Locations.Dispose();
    }
}
