namespace IntegrationMocks.Core.Miscellaneous;

public static class WeakDisposeEventHandler
{
    public static EventHandler Create(IDisposable target)
    {
        var reference = new WeakReference(target);
        return (_, _) => (reference.Target as IDisposable)?.Dispose();
    }
}
