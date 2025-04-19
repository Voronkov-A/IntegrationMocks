using AutoFixture;
using System;
using Xunit;

namespace IntegrationMocks.Modules.Postgres.Tests;

public sealed class EnvironmentPostgresServiceTests
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
        var variables = _fixture.Create<PostgresEnvironmentVariables>();
        Environment.SetEnvironmentVariable(variables.Username.Name, username);
        Environment.SetEnvironmentVariable(variables.Password.Name, password);
        Environment.SetEnvironmentVariable(variables.Host.Name, host);
        Environment.SetEnvironmentVariable(variables.Port.Name, port.ToString());

        using var sut = new EnvironmentPostgresService(variables);

        Assert.Equal(username, sut.Contract.Username);
        Assert.Equal(password, sut.Contract.Password);
        Assert.Equal(host, sut.Contract.Host);
        Assert.Equal(port, sut.Contract.Port);
    }
}
