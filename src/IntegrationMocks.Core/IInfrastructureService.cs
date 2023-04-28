namespace IntegrationMocks.Core;

public interface IInfrastructureService<out TContract> : IDisposable, IAsyncDisposable, IAsyncConstructable
{
    TContract Contract { get; }
}
