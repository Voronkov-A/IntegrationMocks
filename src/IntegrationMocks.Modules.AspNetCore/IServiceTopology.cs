using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.AspNetCore;

public interface IServiceTopology : IAsyncDisposable, IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
