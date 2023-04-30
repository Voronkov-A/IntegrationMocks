namespace IntegrationMocks.Core.Miscellaneous;

public static class TimeServiceExtensions
{
    public static async Task WaitUntilAsync(
        this ITimeService self,
        Func<bool> predicate,
        CancellationToken cancellationToken)
    {
        var period = TimeSpan.FromMilliseconds(500);

        while (!predicate())
        {
            await self.Delay(period, cancellationToken);
        }
    }

    public static async Task WaitUntilAsync(
        this ITimeService self,
        Func<Task<bool>> predicate,
        CancellationToken cancellationToken)
    {
        var period = TimeSpan.FromMilliseconds(500);

        while (!await predicate())
        {
            await self.Delay(period, cancellationToken);
        }
    }

    public static void WaitUntil(
        this ITimeService self,
        Func<bool> predicate,
        CancellationToken cancellationToken)
    {
        var period = TimeSpan.FromMilliseconds(500);

        while (!predicate())
        {
            cancellationToken.ThrowIfCancellationRequested();
            self.Sleep(period);
        }
    }
}
