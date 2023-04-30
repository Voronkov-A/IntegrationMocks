using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.Networking;

public interface IPortManager
{
    IPort TakePort(Range<int> portNumberRange);

    void DeleteAllPorts(Func<int, bool> portNumberPredicate);
}
