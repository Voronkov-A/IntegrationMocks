using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using Microsoft.Extensions.Hosting;

namespace IntegrationMocks.Web.Hosting;

public abstract class HostService<TContract> : IInfrastructureService<TContract>
{
    private IHost? _host;
    private int _disposed;

    public void Dispose()
    {
        using (NullSynchronizationContext.Enter())
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        GC.SuppressFinalize(this);
    }

    public abstract TContract Contract { get; }

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
            throw new InvalidOperationException("The service has already been started.");
        }

        _host = CreateHostBuilder().Build();
        await _host.StartAsync(cancellationToken);
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

        if (_host == null)
        {
            return;
        }

        await _host.StopAsync();
        await _host.WaitForShutdownAsync();
        _host.Dispose();
    }
}
