using System.Net.NetworkInformation;
using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.Networking;

public class PortMonitor : IPortMonitor
{
    public ISet<int> GetUsedPorts(Range<int> portRange)
    {
        var usedPorts = new HashSet<int>();
        var properties = IPGlobalProperties.GetIPGlobalProperties();

        foreach (var connection in properties.GetActiveTcpConnections())
        {
            if (connection.State != TcpState.Closed && portRange.Contains(connection.LocalEndPoint.Port))
            {
                usedPorts.Add(connection.LocalEndPoint.Port);
            }
        }

        foreach (var endPoint in properties.GetActiveTcpListeners())
        {
            if (portRange.Contains(endPoint.Port))
            {
                usedPorts.Add(endPoint.Port);
            }
        }

        foreach (var endPoint in properties.GetActiveUdpListeners())
        {
            if (portRange.Contains(endPoint.Port))
            {
                usedPorts.Add(endPoint.Port);
            }
        }

        return usedPorts;
    }
}
