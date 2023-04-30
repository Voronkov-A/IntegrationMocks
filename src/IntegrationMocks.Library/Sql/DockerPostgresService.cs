using System.Net;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.Execution;
using IntegrationMocks.Core.FluentDocker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Library.Sql;

public class DockerPostgresService : DockerInfrastructureService<SqlServiceContract>
{
    private const int InternalPort = 5432;

    private readonly string _image;
    private readonly IPort _port;

    public DockerPostgresService(
        IDockerContainerManager containerManager,
        IPortManager portManager,
        INameGenerator nameGenerator,
        Range<int> portRange,
        DockerPostgresServiceOptions options)
        : base(containerManager, nameGenerator)
    {
        _image = options.Image;
        _port = portManager.TakePort(portRange);
        Contract = new SqlServiceContract(
            options.Username,
            options.Password,
            IPAddress.Loopback.ToString(),
            _port.Number);
    }

    public DockerPostgresService(
        IDockerContainerManager containerManager,
        IPortManager portManager,
        DockerPostgresServiceOptions options)
        : this(
            containerManager,
            portManager,
            new RandomNameGenerator(nameof(DockerPostgresService)),
            PortRange.Default,
            options)
    {
    }

    public DockerPostgresService(DockerPostgresServiceOptions options)
        : this(
            FluentDockerContainerManager.Default,
            PortManager.Default,
            new RandomNameGenerator(nameof(DockerPostgresService)),
            PortRange.Default,
            options)
    {
    }

    public DockerPostgresService(IDockerContainerManager containerManager)
        : this(
            containerManager,
            PortManager.Default,
            new RandomNameGenerator(nameof(DockerPostgresService)),
            PortRange.Default,
            new DockerPostgresServiceOptions())
    {
    }

    public DockerPostgresService() : this(new DockerPostgresServiceOptions())
    {
    }

    public override SqlServiceContract Contract { get; }

    protected override void ConfigureContainer(IDockerContainerBuilder builder)
    {
        builder
            .UseImage(_image)
            .WithEnvironment($"POSTGRES_USER={Contract.Username}", $"POSTGRES_PASSWORD={Contract.Password}")
            .ExposePort(Contract.Port, InternalPort);
    }

    protected override async Task WaitUntilReady(IDockerContainer container, CancellationToken cancellationToken)
    {
        await TimeService.Instance.WaitUntilAsync(
            () => container.State == DockerContainerState.Running,
            cancellationToken);
        await TimeService.Instance.WaitUntilAsync(
            () => container.GetAllProcesses().Any(x => x.Command == "postgres"),
            cancellationToken);
        await TimeService.Instance.WaitUntilAsync(
            async () =>
                (await container.Execute("pg_isready", new[] { "-U", Contract.Username }, cancellationToken))
                .ExitCode == 0,
            cancellationToken);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        await base.DisposeAsync(disposing);

        if (disposing)
        {
            _port.Dispose();
        }
    }
}
