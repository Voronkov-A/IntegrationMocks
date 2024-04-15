using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.AspNetCore;

public abstract class HostService<TContract> : IInfrastructureService<TContract>
{
    private IServiceTopology? _topology;
    private IHost? _host;
    private int _disposed;

    public abstract TContract Contract { get; }

    public void Dispose()
    {
        using (NullSynchronizationContext.Enter())
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        DisposeFlag.Check(ref _disposed, this);

        if (_host != null)
        {
            return;
        }

        try
        {
            _topology = CreateTopology();

            if (_topology != null)
            {
                await _topology.InitializeAsync(cancellationToken);
            }

            _host = CreateHostBuilder().Build();
            await _host.StartAsync(cancellationToken);
        }
        catch when (_topology != null)
        {
            _host?.Dispose();
            _host = null;
            await _topology.DisposeAsync();
            throw;
        }
    }

    ~HostService()
    {
        Dispose();
    }

    protected abstract IHostBuilder CreateHostBuilder();

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (!DisposeFlag.Mark(ref _disposed))
        {
            return;
        }

        if (_host != null)
        {
            await _host.StopAsync();
            await _host.WaitForShutdownAsync();
            _host.Dispose();

            if (_topology != null)
            {
                await _topology.DisposeAsync();
            }
        }
    }

    protected virtual IServiceTopology? CreateTopology()
    {
        return null;
    }
}
