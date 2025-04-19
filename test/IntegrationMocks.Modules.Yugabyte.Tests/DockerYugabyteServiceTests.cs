using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Yugabyte.Tests.Fixtures;
using Npgsql;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Modules.Yugabyte.Tests;

public sealed class DockerYugabyteServiceTests
{
    private readonly IPortManager _portManager;
    private readonly INameGenerator _nameGenerator;
    private readonly DockerYugabyteServiceOptions _options;

    public DockerYugabyteServiceTests()
    {
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _nameGenerator = new RandomNameGenerator(nameof(DockerYugabyteServiceTests));
        _options = new DockerYugabyteServiceOptions();
    }

    [Fact]
    public async Task Constructor_fills_contract_correctly()
    {
        await using var sut = new DockerYugabyteService(_nameGenerator, _portManager, _options);

        Assert.Equal("localhost", sut.Contract.Host);
        Assert.True(sut.Contract.Port > 0);
        Assert.Equal("yugabyte", sut.Contract.Username);
        Assert.Equal("yugabyte", sut.Contract.Password);
    }

    [Fact]
    public async Task Constructor_does_not_start_service()
    {
        await using var sut = new DockerYugabyteService(_nameGenerator, _portManager, _options);

        var ping = await Ping(sut.CreateYugabyteConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_service_available()
    {
        await using var sut = new DockerYugabyteService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();

        var ping = await Ping(sut.CreateYugabyteConnectionString());
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new DockerYugabyteService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();
        var firstPort = sut.Contract.Port;
        await sut.InitializeAsync();
        var secondPort = sut.Contract.Port;

        var ping = await Ping(sut.CreateYugabyteConnectionString());
        Assert.True(ping);
        Assert.Equal(firstPort, secondPort);
    }

    [Fact]
    public async Task DisposeAsync_makes_service_unavailable()
    {
        await using var sut = new DockerYugabyteService(
            new RandomNameGenerator(nameof(DockerYugabyteServiceTests)),
            _portManager,
            new DockerYugabyteServiceOptions
            {
                PortRange = new Range<int>(
                    UniquePorts.DockerYugabyteServiceTests,
                    UniquePorts.DockerYugabyteServiceTests)
            });
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.CreateYugabyteConnectionString());
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
            return result is string version && version.Contains("yugabyte", StringComparison.OrdinalIgnoreCase);
        }
        catch (NpgsqlException ex) when (ex.InnerException is SocketException or IOException)
        {
            return false;
        }
    }
}
