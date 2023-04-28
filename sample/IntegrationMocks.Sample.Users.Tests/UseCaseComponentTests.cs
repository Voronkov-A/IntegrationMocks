using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Sample.Locations.Mocks;
using IntegrationMocks.Sample.Locations.Mocks.Adapters.WebApi;
using IntegrationMocks.Sample.Users.Adapters.WebApi;
using IntegrationMocks.Sample.Users.Tests.Fixtures;
using IntegrationMocks.Web.Hosting;
using Moq;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests;

public class UseCaseComponentTests : IClassFixture<UsersHostFixture>
{
    private readonly IInfrastructureService<DefaultHostServiceContract> _users;
    private readonly IInfrastructureService<LocationsMockContract> _locations;
    private readonly IFixture _fixture;

    public UseCaseComponentTests(UsersHostFixture services)
    {
        _users = services.Users;
        _locations = services.Locations;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_then_get_user()
    {
        using var client = new UsersHttpClient(UriUtils.HttpLocalhost(_users.Contract.WebApiPort));
        var createUserRequest = _fixture.Create<CreateUserRequest>();
        var locationView = _fixture.Build<LocationView>().With(x => x.Id, createUserRequest.LocationId).Create();
        _locations.Contract.LocationsController
            .Setup(x => x.Get(locationView.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(locationView);

        var createUserResponse = await client.Create(createUserRequest);
        var userView = await client.Get(createUserResponse.Id);

        Assert.Equal(createUserRequest.Name, userView.Name);
        Assert.Equal(createUserRequest.LocationId, userView.LocationId);
    }
}
