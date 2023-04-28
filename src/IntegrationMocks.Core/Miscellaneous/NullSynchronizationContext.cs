namespace IntegrationMocks.Core.Miscellaneous;

public sealed class NullSynchronizationContext : IDisposable
{
    private readonly SynchronizationContext? _synchronizationContext;

    public NullSynchronizationContext()
    {
        _synchronizationContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
    }

    public void Dispose()
    {
        SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
    }

    public static NullSynchronizationContext Enter()
    {
        return new NullSynchronizationContext();
    }
}
