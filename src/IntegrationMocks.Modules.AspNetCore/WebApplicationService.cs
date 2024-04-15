using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.AspNetCore;

public abstract class WebApplicationService<TContract> : IInfrastructureService<TContract>
{
    private IServiceTopology? _topology;
    private WebApplication? _webApplication;
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

        if (_webApplication != null)
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

            _webApplication = CreateWebApplicationBuilder().Build();
            Configure(_webApplication);
            await _webApplication.StartAsync(cancellationToken);
        }
        catch when (_topology != null)
        {
            if (_webApplication != null)
            {
                await _webApplication.DisposeAsync();
            }

            _webApplication = null;
            await _topology.DisposeAsync();
            throw;
        }
    }

    ~WebApplicationService()
    {
        Dispose();
    }

    protected abstract WebApplicationBuilder CreateWebApplicationBuilder();

    protected abstract void Configure(WebApplication app);

    protected virtual ValueTask CreateTopology(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask DropTopology()
    {
        return ValueTask.CompletedTask;
    }

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

        if (_webApplication != null)
        {
            await _webApplication.StopAsync();
            await _webApplication.WaitForShutdownAsync();
            await _webApplication.DisposeAsync();

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
