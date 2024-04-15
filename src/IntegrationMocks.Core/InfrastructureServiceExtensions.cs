using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Core;

public static class InfrastructureServiceExtensions
{
    public static async Task InitializeAsync<T>(this IInfrastructureService<T> self)
    {
        await self.InitializeAsync(CancellationToken.None);
    }
}
