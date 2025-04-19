using IntegrationMocks.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Linq;

namespace IntegrationMocks.Core.Networking;

public sealed class PortManager : IPortManager
{
    private const int RetryCount = 10;

    public static readonly string DefaultPortNumberRepositoryDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        $"{nameof(IntegrationMocks)}_{nameof(PortManager)}");
    public static readonly PortManager Default = new(
        new DirectoryPortNumberRepository(DefaultPortNumberRepositoryDirectoryPath),
        new PortMonitor(),
        NullLogger<PortManager>.Instance);

    private readonly IPortNumberRepository _portNumberRepository;
    private readonly IPortMonitor _portMonitor;
    private readonly ILogger<PortManager> _logger;

    public PortManager(
        IPortNumberRepository portNumberRepository,
        IPortMonitor portMonitor,
        ILogger<PortManager> logger)
    {
        _portNumberRepository = portNumberRepository;
        _portMonitor = portMonitor;
        _logger = logger;
    }

    public PortManager(ILogger<PortManager> logger)
        : this(
              new DirectoryPortNumberRepository(DefaultPortNumberRepositoryDirectoryPath),
              new PortMonitor(),
              logger)
    {
    }

    public IPort TakePort(Range<int> portNumberRange)
    {
        if (portNumberRange.Min <= 0)
        {
            throw new ArgumentException(
                $"Port number {portNumberRange.Min} is negative.",
                nameof(portNumberRange));
        }

        return new PortHandle(this, ref portNumberRange);
    }

    private int CreatePortNumber(ref Range<int> portNumberRange)
    {
        _logger.LogDebug("Locking port from range {@portNumberRange}.", portNumberRange);

        for (var i = 0; i < RetryCount; ++i)
        {
            if (TryCreatePortNumber(ref portNumberRange, out var port))
            {
                _logger.LogDebug(
                    "Locked port {@portNumber} from range {@portNumberRange}.",
                    port,
                    portNumberRange);
                return port;
            }
        }

        throw new InvalidOperationException($"Could not find free port within {portNumberRange}.");
    }

    private bool TryCreatePortNumber(ref Range<int> portNumberRange, out int portNumber)
    {
        var usedPorts = _portNumberRepository
            .GetAll()
            .Concat(_portMonitor.GetUsedPorts(portNumberRange))
            .ToHashSet();

        for (var port = portNumberRange.Min; port <= portNumberRange.Max; ++port)
        {
            if (!usedPorts.Contains(port) && _portNumberRepository.Add(port))
            {
                portNumber = port;
                return true;
            }
        }

        portNumber = 0;
        return false;
    }

    private void DeletePortNumber(int port)
    {
        _logger.LogDebug("Releasing port {@portNumber}.", port);
        _portNumberRepository.Remove(port);
    }

    private sealed class PortHandle : IPort
    {
        private readonly PortManager _manager;

        public PortHandle(PortManager manager, ref Range<int> portNumberRange)
        {
            _manager = manager;
            Number = _manager.CreatePortNumber(ref portNumberRange);
        }

        public int Number { get; }

        public void Dispose()
        {
            _manager.DeletePortNumber(Number);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        ~PortHandle()
        {
            Dispose();
        }
    }
}
