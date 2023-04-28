namespace IntegrationMocks.Core;

public static class AsyncConstructableExtensions
{
    public static async Task InitializeAsync(this IAsyncConstructable self)
    {
        await self.InitializeAsync(CancellationToken.None);
    }
}
