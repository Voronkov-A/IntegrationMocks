namespace IntegrationMocks.Core.Miscellaneous;

public static class TimeServiceExtensions
{
    public static async Task WaitUntil(
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

    public static async Task WaitUntil(
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
}
