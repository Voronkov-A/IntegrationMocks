using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core;

public class DecoratingInfrastructureService<TContract> : IInfrastructureService<TContract>
{
    private readonly IInfrastructureService<TContract> _inner;

    public DecoratingInfrastructureService(IInfrastructureService<TContract> inner)
    {
        _inner = inner;
    }

    public TContract Contract => _inner.Contract;

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

    public virtual async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _inner.InitializeAsync(cancellationToken);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await _inner.DisposeAsync();
        }
    }
}
