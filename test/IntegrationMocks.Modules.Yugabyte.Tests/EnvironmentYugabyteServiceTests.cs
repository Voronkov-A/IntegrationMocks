using System;
using AutoFixture;
using Xunit;

namespace IntegrationMocks.Modules.Yugabyte.Tests;

public sealed class EnvironmentYugabyteServiceTests
{
    private readonly IFixture _fixture;

    public EnvironmentYugabyteServiceTests()
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
        var variables = _fixture.Create<YugabyteEnvironmentVariables>();
        Environment.SetEnvironmentVariable(variables.Username.Name, username);
        Environment.SetEnvironmentVariable(variables.Password.Name, password);
        Environment.SetEnvironmentVariable(variables.Host.Name, host);
        Environment.SetEnvironmentVariable(variables.Port.Name, port.ToString());

        using var sut = new EnvironmentYugabyteService(variables);

        Assert.Equal(username, sut.Contract.Username);
        Assert.Equal(password, sut.Contract.Password);
        Assert.Equal(host, sut.Contract.Host);
        Assert.Equal(port, sut.Contract.Port);
    }
}
