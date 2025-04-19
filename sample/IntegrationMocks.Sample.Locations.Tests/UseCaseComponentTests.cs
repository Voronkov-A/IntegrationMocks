using AutoFixture;
using IntegrationMocks.Sample.Locations.Adapters.WebApi;
using IntegrationMocks.Sample.Locations.Tests.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Sample.Locations.Tests;

public sealed class UseCaseComponentTests : IClassFixture<LocationsHostFixture>
{
    private readonly LocationsHostFixture _host;
    private readonly IFixture _fixture;

    public UseCaseComponentTests(LocationsHostFixture host)
    {
        _host = host;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_then_get_location()
    {
        using var client = new LocationsHttpClient(_host.Locations.Contract.WebApiUrl);
        var createLocationRequest = _fixture.Create<CreateLocationRequest>();

        var createLocationResponse = await client.Create(createLocationRequest);
        var locationView = await client.Get(createLocationResponse.Id);

        Assert.Equal(createLocationRequest.Name, locationView.Name);
    }
}
