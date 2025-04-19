using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using Testcontainers.MySql;

namespace IntegrationMocks.Modules.MySql;

public sealed class DockerMySqlService : IInfrastructureService<MySqlServiceContract>
{
    private readonly IPort _port;
    private readonly MySqlContainer _container;

    public DockerMySqlService()
        : this(new RandomNameGenerator(nameof(DockerMySqlService)), PortManager.Default)
    {
    }

    public DockerMySqlService(INameGenerator nameGenerator, IPortManager portManager)
        : this(nameGenerator, portManager, new DockerMySqlServiceOptions())
    {
    }

    public DockerMySqlService(
        INameGenerator nameGenerator,
        IPortManager portManager,
        DockerMySqlServiceOptions options)
    {
        try
        {
            _port = portManager.TakePort(options.PortRange);

            Contract = new MySqlServiceContract
            {
                Username = "root",
                Password = options.Password,
                Host = "localhost",
                Port = _port.Number
            };

            var builder = new MySqlBuilder()
                .WithImage(options.Image)
                .WithName(nameGenerator.GenerateName())
                .WithPortBinding(_port.Number, MySqlBuilder.MySqlPort)
                .WithEnvironment("MYSQL_ROOT_PASSWORD", options.Password)
                .WithAutoRemove(true);

            if (options.OutputConsumer != null)
            {
                builder = builder.WithOutputConsumer(options.OutputConsumer);
            }

            _container = builder.Build();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public MySqlServiceContract Contract { get; }

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

    ~DockerMySqlService()
    {
        Dispose();
    }
}
