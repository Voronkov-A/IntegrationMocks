using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.MySql.Tests.Fixtures;
using MySql.Data.MySqlClient;
using Xunit;

namespace IntegrationMocks.Modules.MySql.Tests;

public sealed class DockerMySqlServiceTests
{
    private readonly IPortManager _portManager;
    private readonly INameGenerator _nameGenerator;
    private readonly DockerMySqlServiceOptions _options;

    public DockerMySqlServiceTests()
    {
        var fixture = new Fixture();
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _nameGenerator = new RandomNameGenerator(nameof(DockerMySqlServiceTests));
        _options = new DockerMySqlServiceOptions
        {
            Password = fixture.Create<string>()
        };
    }

    [Fact]
    public async Task Constructor_fills_contract_correctly()
    {
        await using var sut = new DockerMySqlService(_nameGenerator, _portManager, _options);

        Assert.Equal("localhost", sut.Contract.Host);
        Assert.True(sut.Contract.Port > 0);
        Assert.Equal("root", sut.Contract.Username);
        Assert.Equal(_options.Password, sut.Contract.Password);
    }

    [Fact]
    public async Task Constructor_does_not_start_service()
    {
        await using var sut = new DockerMySqlService(_nameGenerator, _portManager, _options);

        var ping = await Ping(sut.CreateMySqlConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_service_available()
    {
        await using var sut = new DockerMySqlService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();

        var ping = await Ping(sut.CreateMySqlConnectionString());
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new DockerMySqlService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();
        var firstPort = sut.Contract.Port;
        await sut.InitializeAsync();
        var secondPort = sut.Contract.Port;

        var ping = await Ping(sut.CreateMySqlConnectionString());
        Assert.True(ping);
        Assert.Equal(firstPort, secondPort);
    }

    [Fact]
    public async Task DisposeAsync_makes_service_unavailable()
    {
        await using var sut = new DockerMySqlService(
            new RandomNameGenerator(nameof(DockerMySqlServiceTests)),
            _portManager,
            new DockerMySqlServiceOptions
            {
                PortRange = new Range<int>(
                    UniquePorts.DockerMySqlServiceTests,
                    UniquePorts.DockerMySqlServiceTests)
            });
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.CreateMySqlConnectionString());
        Assert.False(ping);
    }

    private static async Task<bool> Ping(string connectionString)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "select version();";
            var result = await command.ExecuteScalarAsync();
            return result is "8.4.5";
        }
        catch (MySqlException ex) when (ex.InnerException is SocketException or IOException)
        {
            return false;
        }
    }
}
