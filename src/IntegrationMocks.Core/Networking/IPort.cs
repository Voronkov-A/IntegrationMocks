namespace IntegrationMocks.Core.Networking;

public interface IPort : IDisposable
{
    int Number { get; }
}
