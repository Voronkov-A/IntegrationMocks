namespace IntegrationMocks.Core.Miscellaneous;

public class TimeService : ITimeService
{
    public static readonly TimeService Instance = new();

    private TimeService()
    {
    }

    public async Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
    }
}
