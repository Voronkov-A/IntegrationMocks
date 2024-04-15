using IntegrationMocks.Core.Miscellaneous;
using System.Collections.Generic;

namespace IntegrationMocks.Core.Networking;

public interface IPortMonitor
{
    ISet<int> GetUsedPorts(Range<int> portRange);
}
