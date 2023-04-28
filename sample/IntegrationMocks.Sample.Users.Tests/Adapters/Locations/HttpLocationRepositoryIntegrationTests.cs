using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Sample.Locations.Mocks;
using IntegrationMocks.Sample.Locations.Mocks.Adapters.WebApi;
using IntegrationMocks.Sample.Users.Adapters.Locations;
using IntegrationMocks.Sample.Users.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Adapters.Locations;

public class HttpLocationRepositoryIntegrationTests : IClassFixture<LocationsMockFixture>
{
    private readonly IInfrastructureService<LocationsMockContract> _locations;
    private readonly IFixture _fixture;
    private readonly HttpLocationRepository _sut;

    public HttpLocationRepositoryIntegrationTests(LocationsMockFixture services)
    {
        _locations = services.Locations;
        _fixture = new Fixture();
        _sut = new HttpLocationRepository(new LocationsOptions()
        {
            BaseAddress = UriUtils.HttpLocalhost(_locations.Contract.WebApiPort)
        });
    }

    [Fact]
    public async Task Find_returns_existing_location()
    {
        var locationView = _fixture.Create<LocationView>();
        var expectedLocationId = locationView.Id;
        _locations.Contract.LocationsController
            .Setup(x => x.Get(expectedLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(locationView);

        var actualLocation = await _sut.Find(expectedLocationId, CancellationToken.None);

        Assert.NotNull(actualLocation);
        Assert.Equal(expectedLocationId, actualLocation.Value.Id);
    }

    [Fact]
    public async Task Find_returns_null_for_nonexistent_location()
    {
        var locationId = _fixture.Create<Guid>();
        _locations.Contract.LocationsController
            .Setup(x => x.Get(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotFoundResult());

        var actualLocation = await _sut.Find(locationId, CancellationToken.None);

        Assert.Null(actualLocation);
    }
}
