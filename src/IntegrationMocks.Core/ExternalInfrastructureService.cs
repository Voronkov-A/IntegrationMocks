using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core;

public class ExternalInfrastructureService<TContract> : IInfrastructureService<TContract>
{
    public ExternalInfrastructureService(TContract contract)
    {
        Contract = contract;
    }

    public TContract Contract { get; }

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

    public virtual Task InitializeAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        return ValueTask.CompletedTask;
    }
}
