using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Postgres.Tests.Fixtures;
using Npgsql;
using System.Data.Common;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Modules.Postgres.Tests;

public sealed class DockerPostgresServiceTests
{
    private readonly IFixture _fixture;
    private readonly IPortManager _portManager;
    private readonly INameGenerator _nameGenerator;
    private readonly DockerPostgresServiceOptions _options;

    public DockerPostgresServiceTests()
    {
        _fixture = new Fixture();
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _nameGenerator = new RandomNameGenerator(nameof(DockerPostgresServiceTests));
        _options = new DockerPostgresServiceOptions
        {
            Username = _fixture.Create<string>(),
            Password = _fixture.Create<string>()
        };
    }

    [Fact]
    public async Task Constructor_fills_contract_correctly()
    {
        await using var sut = new DockerPostgresService(_nameGenerator, _portManager, _options);

        Assert.Equal("localhost", sut.Contract.Host);
        Assert.True(sut.Contract.Port > 0);
        Assert.Equal(_options.Username, sut.Contract.Username);
        Assert.Equal(_options.Password, sut.Contract.Password);
    }

    [Fact]
    public async Task Constructor_does_not_start_service()
    {
        await using var sut = new DockerPostgresService(_nameGenerator, _portManager, _options);

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_service_available()
    {
        await using var sut = new DockerPostgresService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new DockerPostgresService(_nameGenerator, _portManager, _options);

        await sut.InitializeAsync();
        var firstPort = sut.Contract.Port;
        await sut.InitializeAsync();
        var secondPort = sut.Contract.Port;

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.True(ping);
        Assert.Equal(firstPort, secondPort);
    }

    [Fact]
    public async Task DisposeAsync_makes_service_unavailable()
    {
        await using var sut = new DockerPostgresService(
            new RandomNameGenerator(nameof(DockerPostgresServiceTests)),
            _portManager,
            new DockerPostgresServiceOptions
            {
                PortRange = new Range<int>(
                    UniquePorts.DockerPostgresServiceTests,
                    UniquePorts.DockerPostgresServiceTests)
            });
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.CreatePostgresConnectionString());
        Assert.False(ping);
    }

    [Fact]
    public async Task Can_create_table()
    {
        await using var sut = new DockerPostgresService(_nameGenerator, _portManager, _options);
        await sut.InitializeAsync();

        var databaseName = _fixture.Create<string>();

        await using var masterConnection = new NpgsqlConnection(sut.CreatePostgresConnectionString());
        await masterConnection.OpenAsync();
        await ExecuteNonQueryAsync(masterConnection, $"CREATE DATABASE \"{databaseName}\";");

        await using var connection = new NpgsqlConnection(sut.CreatePostgresConnectionString(databaseName));
        await connection.OpenAsync();
        await ExecuteNonQueryAsync(connection, "CREATE TABLE test (id text);");
        var inserted = await ExecuteNonQueryAsync(connection, "INSERT INTO test (id) VALUES ('id');");

        Assert.Equal(1, inserted);
    }

    private static async Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        return await command.ExecuteNonQueryAsync();
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
