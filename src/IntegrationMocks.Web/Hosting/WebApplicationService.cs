using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace IntegrationMocks.Web.Hosting;

public abstract class WebApplicationService<TContract> : IInfrastructureService<TContract>
{
    private WebApplication? _webApplication;
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

        if (_webApplication != null)
        {
            throw new InvalidOperationException("The service has already been started.");
        }

        _webApplication = CreateWebApplicationBuilder().Build();
        Configure(_webApplication);
        await _webApplication.StartAsync(cancellationToken);
    }

    protected abstract WebApplicationBuilder CreateWebApplicationBuilder();

    protected abstract void Configure(WebApplication application);

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

        if (_webApplication == null)
        {
            return;
        }

        await _webApplication.StopAsync();
        await _webApplication.WaitForShutdownAsync();
        await _webApplication.DisposeAsync();
    }
}
