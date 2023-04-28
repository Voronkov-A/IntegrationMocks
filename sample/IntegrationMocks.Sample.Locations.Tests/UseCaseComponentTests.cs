using AutoFixture;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Sample.Locations.Adapters.WebApi;
using IntegrationMocks.Sample.Locations.Tests.Fixtures;
using Xunit;

namespace IntegrationMocks.Sample.Locations.Tests;

public class UseCaseComponentTests : IClassFixture<LocationsHostFixture>
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
        using var client = new LocationsHttpClient(UriUtils.HttpLocalhost(_host.Locations.Contract.WebApiPort));
        var createLocationRequest = _fixture.Create<CreateLocationRequest>();

        var createLocationResponse = await client.Create(createLocationRequest);
        var locationView = await client.Get(createLocationResponse.Id);

        Assert.Equal(createLocationRequest.Name, locationView.Name);
    }
}
