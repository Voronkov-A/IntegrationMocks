using System.Net;
using System.Net.Sockets;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Core.Tests.Fixtures;
using Xunit;

namespace IntegrationMocks.Core.Tests.Networking;

public class PortMonitorTests
{
    private const int Port = UniquePorts.PortMonitorTests;

    private readonly PortMonitor _sut;

    public PortMonitorTests()
    {
        _sut = new PortMonitor();
    }

    [Fact]
    public void GetUsedPorts_does_not_return_free_tcp_ports()
    {
        EnsurePortIsFree(Port);

        var ports = _sut.GetUsedPorts(new Range<int>(Port, Port));

        Assert.Empty(ports);
    }

    [Fact]
    public void GetUsedPorts_returns_busy_tcp_ports()
    {
        var listener = new TcpListener(IPAddress.Loopback, Port);
        try
        {
            listener.Start();

            var ports = _sut.GetUsedPorts(new Range<int>(Port, Port));

            Assert.Equal(new[] { Port }, ports);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void EnsurePortIsFree(int port)
    {
        var listener = new TcpListener(IPAddress.Loopback, port);
        try
        {
            listener.Start();
        }
        finally
        {
            listener.Stop();
        }
    }
}
