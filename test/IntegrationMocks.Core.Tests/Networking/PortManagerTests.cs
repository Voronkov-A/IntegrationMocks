using System.Runtime.CompilerServices;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Core.Resources;
using IntegrationMocks.Core.Tests.Fixtures;
using Moq;
using Xunit;

namespace IntegrationMocks.Core.Tests.Networking;

public class PortManagerTests
{
    private readonly IStringRepository _portRepository;
    private readonly Mock<IPortMonitor> _portMonitorMock;
    private readonly Range<int> _portRange;
    private readonly PortManager _sut;

    public PortManagerTests()
    {
        _portRepository = new InMemoryStringRepository();
        _portMonitorMock = new Mock<IPortMonitor>();
        _portRange = new Range<int>(34243, 34443);
        _sut = new PortManager(_portRepository, _portMonitorMock.Object);
    }

    [Fact]
    public void TakePort_locks_first_free_port()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(new HashSet<int>());
        var expectedPort = _portRange.Min;

        using var port = _sut.TakePort(_portRange);

        Assert.Equal(expectedPort, port.Number);
    }

    [Fact]
    public void TakePort_does_not_take_port_in_use()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(
            new HashSet<int>
            {
                _portRange.Min
            });
        var expectedPort = _portRange.Min + 1;

        using var port = _sut.TakePort(_portRange);

        Assert.Equal(expectedPort, port.Number);
    }

    [Fact]
    public async Task TakePort_is_thread_safe()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(It.IsAny<Range<int>>())).Returns(new HashSet<int>());
        var ports = new List<IPort>(100);
        try
        {
            await Task.WhenAll(
                Enumerable.Range(0, 100).Select(_ => Task.Run(() => ports.Add(_sut.TakePort(_portRange)))));

            Assert.Equal(ports.Count, ports.Select(x => x.Number).Distinct().Count());
        }
        finally
        {
            foreach (var port in ports) port.Dispose();
        }
    }

    [Fact]
    public void TakePort_marks_port_as_used_in_repository()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(new HashSet<int>());

        using var port = _sut.TakePort(_portRange);

        Assert.Contains(port.Number.ToString(), _portRepository.GetAll());
    }

    [Fact]
    public void TakePort_creates_port_that_can_be_destroyed_with_dispose()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(new HashSet<int>());
        using var port = _sut.TakePort(_portRange);
        var portNumber = port.Number;

        port.Dispose();

        Assert.DoesNotContain(portNumber.ToString(), _portRepository.GetAll());
    }

    [Fact]
    public void TakePort_creates_port_that_can_be_destroyed_with_finalizer()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(new HashSet<int>());
        using var port = _sut.TakePort(_portRange);

        var portNumber = CreatePortAndForget(_sut);
        GC.Collect(2);
        GC.WaitForPendingFinalizers();

        Assert.DoesNotContain(portNumber.ToString(), _portRepository.GetAll());
    }

    [Fact]
    public void TakePort_throws_when_all_ports_are_in_use()
    {
        _portMonitorMock
            .Setup(x => x.GetUsedPorts(_portRange))
            .Returns(Enumerable.Range(_portRange.Min, _portRange.Max - _portRange.Min + 1).ToHashSet());

        Assert.Throws<InvalidOperationException>(() => _sut.TakePort(_portRange));
    }

    [Fact]
    public void DeleteAllPorts_clears_repository()
    {
        _portMonitorMock.Setup(x => x.GetUsedPorts(_portRange)).Returns(new HashSet<int>());
        using var port = _sut.TakePort(_portRange);
        var portNumber = port.Number;

        _sut.DeleteAllPorts();

        Assert.DoesNotContain(portNumber.ToString(), _portRepository.GetAll());
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private int CreatePortAndForget(IPortManager manager)
    {
        return manager.TakePort(_portRange).Number;
    }
}
