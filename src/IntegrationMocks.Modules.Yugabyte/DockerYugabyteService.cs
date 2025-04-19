using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.Yugabyte;

public sealed class DockerYugabyteService : IInfrastructureService<YugabyteServiceContract>
{
    private readonly IPort _port;
    private readonly IContainer _container;

    public DockerYugabyteService()
        : this(new RandomNameGenerator(nameof(DockerYugabyteService)), PortManager.Default)
    {
    }

    public DockerYugabyteService(INameGenerator nameGenerator, IPortManager portManager)
        : this(nameGenerator, portManager, new DockerYugabyteServiceOptions())
    {
    }

    public DockerYugabyteService(
        INameGenerator nameGenerator,
        IPortManager portManager,
        DockerYugabyteServiceOptions options)
    {
        try
        {
            _port = portManager.TakePort(options.PortRange);

            Contract = new YugabyteServiceContract
            {
                Username = "yugabyte",
                Password = "yugabyte",
                Host = "localhost",
                Port = _port.Number
            };

            var builder = new ContainerBuilder()
                .WithImage(options.Image)
                .WithName(nameGenerator.GenerateName())
                .WithPortBinding(_port.Number, 5433)
                .WithAutoRemove(true)
                .WithCommand("bin/yugabyted", "start", "--background=false")
                .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitStrategy(Contract)));

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

    public YugabyteServiceContract Contract { get; }

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

    ~DockerYugabyteService()
    {
        Dispose();
    }

    private class WaitStrategy : IWaitUntil
    {
        private readonly YugabyteServiceContract _contract;

        public WaitStrategy(YugabyteServiceContract contract)
        {
            _contract = contract;
        }

        public async Task<bool> UntilAsync(IContainer container)
        {
            try
            {
                var result = await container.ExecAsync(new List<string>
                {
                    "/bin/sh",
                    "-c",
                    $"export PGPASSWORD={_contract.Password} && export HOSTNAME=$(hostname -i) && bin/ysqlsh --host $HOSTNAME --port 5433 --username {_contract.Username} --no-password -c 'select version();' || exit $?"
                });

                return result.ExitCode == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
