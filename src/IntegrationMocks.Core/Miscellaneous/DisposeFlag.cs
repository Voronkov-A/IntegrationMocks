namespace IntegrationMocks.Core.Miscellaneous;

public static class DisposeFlag
{
    public static void Check<T>(ref int disposed, T obj) where T : notnull
    {
        if (Interlocked.CompareExchange(ref disposed, 0, 0) != 0)
        {
            throw new ObjectDisposedException(obj.GetType().ToString());
        }
    }

    public static bool Mark(ref int disposed)
    {
        return Interlocked.Exchange(ref disposed, 1) == 0;
    }
}
