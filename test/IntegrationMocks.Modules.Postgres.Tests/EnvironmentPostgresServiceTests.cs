using AutoFixture;
using System;
using Xunit;

namespace IntegrationMocks.Modules.Postgres.Tests;

public class EnvironmentPostgresServiceTests
{
    private readonly IFixture _fixture;

    public EnvironmentPostgresServiceTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_builds_contract_from_environment_variables()
    {
        var username = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var host = _fixture.Create<string>();
        var port = _fixture.Create<int>() % 100 + 33000;
        Environment.SetEnvironmentVariable("SqlServiceContract_Username", username);
        Environment.SetEnvironmentVariable("SqlServiceContract_Password", password);
        Environment.SetEnvironmentVariable("SqlServiceContract_Host", host);
        Environment.SetEnvironmentVariable("SqlServiceContract_Port", port.ToString());

        using var sut = new EnvironmentPostgresService();

        Assert.Equal(username, sut.Contract.Username);
        Assert.Equal(password, sut.Contract.Password);
        Assert.Equal(host, sut.Contract.Host);
        Assert.Equal(port, sut.Contract.Port);
    }
}
