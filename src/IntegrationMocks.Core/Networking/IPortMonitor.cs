using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.Networking;

public interface IPortMonitor
{
    ISet<int> GetUsedPorts(Range<int> portRange);
}
