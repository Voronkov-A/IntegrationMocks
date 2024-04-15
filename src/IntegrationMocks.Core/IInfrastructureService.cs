using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Core;

public interface IInfrastructureService<out TContract> : IDisposable, IAsyncDisposable
{
    TContract Contract { get; }

    Task InitializeAsync(CancellationToken cancellationToken);
}
