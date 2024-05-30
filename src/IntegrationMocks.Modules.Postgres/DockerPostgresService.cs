using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Sql;
using IntegrationMocks.Modules.Testcontainers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace IntegrationMocks.Modules.Postgres;

public sealed class DockerPostgresService : IInfrastructureService<SqlServiceContract>
{
    private readonly IPort _port;
    private readonly string _name;
    private readonly PostgreSqlContainer _container;

    public DockerPostgresService()
        : this(new RandomNameGenerator(nameof(DockerPostgresService)), PortManager.Default)
    {
    }

    public DockerPostgresService(INameGenerator nameGenerator, IPortManager portManager)
        : this(nameGenerator, portManager, PortRange.Default, new DockerPostgresServiceOptions())
    {
    }

    public DockerPostgresService(
        INameGenerator nameGenerator,
        IPortManager portManager,
        Range<int> portRange,
        DockerPostgresServiceOptions options)
    {
        _port = portManager.TakePort(portRange);
        _name = nameGenerator.GenerateName();
        _container = new PostgreSqlBuilder()
            .WithImage(options.Image)
            .WithName(_name)
            .WithPortBinding(_port.Number, PostgreSqlBuilder.PostgreSqlPort)
            .WithEnvironment("POSTGRES_USER", options.Username)
            .WithEnvironment("POSTGRES_PASSWORD", options.Password)
            .WithAutoRemove(true)
            .Build();

        Contract = new SqlServiceContract(
            username: options.Username,
            password: options.Password,
            host: "localhost",
            port: _port.Number);
    }

    public SqlServiceContract Contract { get; }

    public async ValueTask DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }

        _port?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _container.StartOrInspectAsync(_name, cancellationToken);
    }

    public void Dispose()
    {
        using (NullSynchronizationContext.Enter())
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        GC.SuppressFinalize(this);
    }

    ~DockerPostgresService()
    {
        Dispose();
    }
}
