using System.Runtime.InteropServices;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.Networking;

public class PortManager : IPortManager
{
    public static readonly string DefaultPortNumberRepositoryDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        $"{nameof(IntegrationMocks)}_{nameof(PortManager)}");
    public static readonly PortManager Default = new(
        new DirectoryStringRepository(DefaultPortNumberRepositoryDirectoryPath),
        new PortMonitor(),
        new Range<int>(33000, 34000));

    private readonly IStringRepository _portNumberRepository;
    private readonly IPortMonitor _portMonitor;
    private readonly Range<int> _portRange;

    public PortManager(IStringRepository portNumberRepository, IPortMonitor portMonitor, Range<int> portRange)
    {
        _portNumberRepository = portNumberRepository;
        _portMonitor = portMonitor;
        _portRange = portRange;
    }

    public IPort TakePort()
    {
        return new PortHandle(this);
    }

    public void DeleteAllPorts()
    {
        foreach (var portNumber in _portNumberRepository.GetAll())
        {
            _portNumberRepository.Remove(portNumber);
        }
    }

    private int CreatePortNumber()
    {
        var usedPorts = _portNumberRepository
            .GetAll()
            .Select(int.Parse)
            .Concat(_portMonitor.GetUsedPorts(_portRange))
            .ToHashSet();

        for (var port = _portRange.Min; port <= _portRange.Max; ++port)
        {
            if (!usedPorts.Contains(port) && _portNumberRepository.Add(port.ToString()))
            {
                return port;
            }
        }

        throw new InvalidOperationException($"Could not find free port within {_portRange}.");
    }

    private void DeletePortNumber(int port)
    {
        _portNumberRepository.Remove(port.ToString());
    }

    private class PortHandle : SafeHandle, IPort
    {
        private readonly PortManager _manager;
        private readonly EventHandler _processExitHook;

        public PortHandle(PortManager manager) : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            Number = _manager.CreatePortNumber();
        }

        public int Number { get; }

        public override bool IsInvalid => false;

        public override string ToString()
        {
            return Number.ToString();
        }

        protected override bool ReleaseHandle()
        {
            _manager.DeletePortNumber(Number);
            AppDomain.CurrentDomain.ProcessExit -= _processExitHook;
            return true;
        }
    }
}