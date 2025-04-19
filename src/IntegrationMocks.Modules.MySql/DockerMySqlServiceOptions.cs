using DotNet.Testcontainers.Configurations;
using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Modules.MySql;

public sealed class DockerMySqlServiceOptions
{
    public string Password { get; init; } = "mysql";

    public string Image { get; init; } = "mysql:8.4.5";

    public Range<int> PortRange { get; init; } = Core.Networking.PortRange.Default;

    public IOutputConsumer? OutputConsumer { get; init; }
}
