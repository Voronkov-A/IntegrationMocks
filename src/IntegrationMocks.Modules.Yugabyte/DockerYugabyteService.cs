using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Names;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.Sql;
using IntegrationMocks.Modules.Testcontainers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.Yugabyte;

public class DockerYugabyteService : IInfrastructureService<SqlServiceContract>
{
    private readonly IPort _port;
    private readonly IContainer _container;

    public DockerYugabyteService()
        : this(new RandomNameGenerator(nameof(DockerYugabyteService)), PortManager.Default)
    {
    }

    public DockerYugabyteService(INameGenerator nameGenerator, IPortManager portManager)
        : this(nameGenerator, portManager, PortRange.Default, new DockerYugabyteServiceOptions(), false)
    {
    }

    public DockerYugabyteService(INameGenerator nameGenerator, IPortManager portManager, bool attachOutput)
        : this(nameGenerator, portManager, PortRange.Default, new DockerYugabyteServiceOptions(), attachOutput)
    {
    }

    public DockerYugabyteService(
        INameGenerator nameGenerator,
        IPortManager portManager,
        Range<int> portRange,
        DockerYugabyteServiceOptions options,
        bool attachOutput)
    {
        _port = portManager.TakePort(portRange);

        Contract = new SqlServiceContract(
            username: "yugabyte",
            password: "yugabyte",
            host: "localhost",
            port: _port.Number);

        _container = new ContainerBuilder()
            .WithImage(options.Image)
            .WithName(nameGenerator.GenerateName())
            .WithPortBinding(_port.Number, 5433)
            .WithAutoRemove(true)
            .WithOutput<ContainerBuilder, IContainer>(attachOutput)
            .WithCommand("bin/yugabyted", "start", "--background=false")
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitStrategy(Contract)))
            .Build();
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
        private readonly SqlServiceContract _contract;

        public WaitStrategy(SqlServiceContract contract)
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
