namespace IntegrationMocks.Core.Miscellaneous;

public interface ITimeService
{
    Task Delay(TimeSpan delay, CancellationToken cancellationToken);

    void Sleep(TimeSpan delay);
}
