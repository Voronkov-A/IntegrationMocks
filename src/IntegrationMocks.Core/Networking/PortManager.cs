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
        new PortMonitor());

    private readonly IStringRepository _portNumberRepository;
    private readonly IPortMonitor _portMonitor;

    public PortManager(IStringRepository portNumberRepository, IPortMonitor portMonitor)
    {
        _portNumberRepository = portNumberRepository;
        _portMonitor = portMonitor;
    }

    public IPort TakePort(Range<int> portNumberRange)
    {
        if (portNumberRange.Min <= 0)
        {
            throw new ArgumentException($"Port number {portNumberRange.Min} is negative.", nameof(portNumberRange));
        }

        return new PortHandle(this, ref portNumberRange);
    }

    public void DeleteAllPorts(Func<int, bool> portNumberPredicate)
    {
        foreach (var portNumber in _portNumberRepository.GetAll().Where(x => portNumberPredicate(int.Parse(x))))
        {
            _portNumberRepository.Remove(portNumber);
        }
    }

    private int CreatePortNumber(ref Range<int> portNumberRange)
    {
        var usedPorts = _portNumberRepository
            .GetAll()
            .Select(int.Parse)
            .Concat(_portMonitor.GetUsedPorts(portNumberRange))
            .ToHashSet();

        for (var port = portNumberRange.Min; port <= portNumberRange.Max; ++port)
        {
            if (!usedPorts.Contains(port) && _portNumberRepository.Add(port.ToString()))
            {
                return port;
            }
        }

        throw new InvalidOperationException($"Could not find free port within {portNumberRange}.");
    }

    private void DeletePortNumber(int port)
    {
        _portNumberRepository.Remove(port.ToString());
    }

    private class PortHandle : SafeHandle, IPort
    {
        private readonly PortManager _manager;
        private readonly EventHandler _processExitHook;

        public PortHandle(PortManager manager, ref Range<int> portNumberRange) : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            Number = _manager.CreatePortNumber(ref portNumberRange);
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