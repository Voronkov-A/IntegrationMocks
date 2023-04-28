namespace IntegrationMocks.Core;

public interface IAsyncConstructable
{
    /// <summary>
    /// Asynchronous constructor. Should be idempotent.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InitializeAsync(CancellationToken cancellationToken);
}
