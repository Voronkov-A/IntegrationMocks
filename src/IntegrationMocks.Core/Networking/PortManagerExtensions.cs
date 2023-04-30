namespace IntegrationMocks.Core.Networking;

public static class PortManagerExtensions
{
    public static IPort TakePort(this IPortManager self)
    {
        return self.TakePort(PortRange.Default);
    }

    public static void DeleteAllPorts(this IPortManager self)
    {
        self.DeleteAllPorts(_ => true);
    }
}
