using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace IntegrationMocks.Modules.Postgres;

public sealed class DockerPostgresService : IInfrastructureService<PostgresServiceContract>
{
    private readonly IPort _port;
    private readonly PostgreSqlContainer _container;

    public DockerPostgresService()
        : this(new RandomNameGenerator(nameof(DockerPostgresService)), PortManager.Default)
    {
    }

    public DockerPostgresService(INameGenerator nameGenerator, IPortManager portManager)
        : this(nameGenerator, portManager, new DockerPostgresServiceOptions())
    {
    }

    public DockerPostgresService(
        INameGenerator nameGenerator,
        IPortManager portManager,
        DockerPostgresServiceOptions options)
    {
        try
        {
            _port = portManager.TakePort(options.PortRange);
            var builder = new PostgreSqlBuilder()
                .WithImage(options.Image)
                .WithName(nameGenerator.GenerateName())
                .WithPortBinding(_port.Number, PostgreSqlBuilder.PostgreSqlPort)
                .WithEnvironment("POSTGRES_USER", options.Username)
                .WithEnvironment("POSTGRES_PASSWORD", options.Password)
                .WithAutoRemove(true);

            if (options.OutputConsumer != null)
            {
                builder = builder.WithOutputConsumer(options.OutputConsumer);
            }

            _container = builder.Build();

            Contract = new PostgresServiceContract
            {
                Username = options.Username,
                Password = options.Password,
                Host = "localhost",
                Port = _port.Number
            };
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public PostgresServiceContract Contract { get; }

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
        await _container.StartAsync(cancellationToken);
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
