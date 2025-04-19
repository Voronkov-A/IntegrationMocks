using System;
using System.Threading;

namespace IntegrationMocks.Core.Miscellaneous;

public static class DisposeFlag
{
    public static void Check<T>(ref int disposed, T obj) where T : notnull
    {
        ObjectDisposedException.ThrowIf(Interlocked.CompareExchange(ref disposed, 0, 0) != 0, obj.GetType());
    }

    public static bool Mark(ref int disposed)
    {
        return Interlocked.Exchange(ref disposed, 1) == 0;
    }
}
