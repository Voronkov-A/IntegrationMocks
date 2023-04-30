using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.Networking;

public static class PortRange
{
    public static readonly Range<int> Default = new(33000, 33999);
}
