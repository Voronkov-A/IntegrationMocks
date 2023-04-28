namespace IntegrationMocks.Core.Networking;

public interface IPortManager
{
    IPort TakePort();

    void DeleteAllPorts();
}
