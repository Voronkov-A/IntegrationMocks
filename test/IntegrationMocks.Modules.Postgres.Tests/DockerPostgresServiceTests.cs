using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Postgres.Tests.Fixtures;
using Npgsql;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Modules.Postgres.Tests;

public class DockerPostgresServiceTests
{
    private readonly IPortManager _portManager;
    private readonly INameGenerator _nameGenerator;
    private readonly DockerPostgresServiceOptions _options;

    public DockerPostgresServiceTests()
    {
        var fixture = new Fixture();
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _nameGenerator = new RandomNameGenerator(nameof(DockerPostgresServiceTests));
        _options = fixture.Build<DockerPostgresServiceOptions>().Without(x => x.Image).Create();
    }

    [Fact]
    public async Task Constructor_fills_contract_correctly()
    {
        await using var sut = new DockerPostgresService(
            _nameGenerator,
            _portManager,
            PortRange.Default,
            _options,
            attachOutput: false);

        Assert.Equal("localhost", sut.Contract.Host);
        Assert.True(sut.Contract.Port > 0);
        Assert.Equal(_options.Username, sut.Contract.Username);
        Assert.Equal(_options.Password, sut.Contract.Password);
    }

    [Fact]
    public async Task Constructor_does_not_start_postgres()
    {
        await using var sut = new DockerPostgresService(
            _nameGenerator,
            _portManager,
            PortRange.Default,
            _options,
            attachOutput: false);

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_postgres_available()
    {
        await using var sut = new DockerPostgresService(
            _nameGenerator,
            _portManager,
            PortRange.Default,
            _options,
            attachOutput: false);

        await sut.InitializeAsync();

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new DockerPostgresService(
            _nameGenerator,
            _portManager,
            PortRange.Default,
            _options,
            attachOutput: false);

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
            portManager: _portManager,
            nameGenerator: new RandomNameGenerator(nameof(DockerPostgresServiceTests)),
            portRange: new Range<int>(
                UniquePorts.DockerPostgresServiceTests,
                UniquePorts.DockerPostgresServiceTests),
            options: _options,
            attachOutput: false);
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
        catch (NpgsqlException ex) when (ex.InnerException is SocketException or IOException)
        {
            return false;
        }
    }
}
