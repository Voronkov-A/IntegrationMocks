using System.Net;
using System.Net.Sockets;
using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.FluentDocker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Core.Resources;
using IntegrationMocks.Library.Sql;
using IntegrationMocks.Library.Tests.Fixtures;
using Npgsql;
using Xunit;

namespace IntegrationMocks.Library.Tests.Sql;

public class DockerPostgresServiceTests
{
    private readonly IDockerContainerManager _dockerContainerManager;
    private readonly IPortManager _portManager;
    private readonly DockerPostgresServiceOptions _options;

    public DockerPostgresServiceTests()
    {
        var fixture = new Fixture();
        _dockerContainerManager = new FluentDockerContainerManager(
            LoggerFixture.CreateLogger<FluentDockerContainerManager>());
        _portManager = PortManager.Default;
        _options = fixture.Build<DockerPostgresServiceOptions>().Without(x => x.Image).Create();
    }

    [Fact]
    public async Task Constructor_fills_contract_correctly()
    {
        await using var sut = new DockerPostgresService(_dockerContainerManager, _portManager, _options);

        Assert.Equal(sut.Contract.Host, IPAddress.Loopback.ToString());
        Assert.True(sut.Contract.Port > 0);
        Assert.Equal(sut.Contract.Username, _options.Username);
        Assert.Equal(sut.Contract.Password, _options.Password);
    }

    [Fact]
    public async Task Constructor_does_not_start_postgres()
    {
        await using var sut = new DockerPostgresService(_dockerContainerManager, _portManager, _options);

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_postgres_available()
    {
        await using var sut = new DockerPostgresService(_dockerContainerManager, _portManager, _options);

        await sut.InitializeAsync();

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new DockerPostgresService(_dockerContainerManager, _portManager, _options);

        await sut.InitializeAsync();
        var firstPort = sut.Contract.Port;
        await sut.InitializeAsync();
        var secondPort = sut.Contract.Port;

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.True(ping);
        Assert.Equal(firstPort, secondPort);
    }

    [Fact]
    public async Task DisposeAsync_makes_postgres_unavailable()
    {
        await using var sut = new DockerPostgresService(
            containerManager: _dockerContainerManager,
            portManager: _portManager,
            nameGenerator: new RandomNameGenerator(nameof(DockerPostgresServiceTests)),
            portRange: new Range<int>(UniquePorts.DockerPostgresServiceTests, UniquePorts.DockerPostgresServiceTests),
            options: _options);
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.False(ping);
    }

    private static async Task<bool> Ping(string connectionString)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "select version();";
            var result = await command.ExecuteScalarAsync();
            return result is string version && version.StartsWith("PostgreSQL ");
        }
        catch (NpgsqlException ex) when (ex.InnerException is SocketException)
        {
            return false;
        }
    }
}
