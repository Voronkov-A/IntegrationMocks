using DotNet.Testcontainers.Configurations;
using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Modules.Yugabyte;

public sealed class DockerYugabyteServiceOptions
{
    public string Image { get; init; } = "yugabytedb/yugabyte:2.25.1.0-b381";

    public Range<int> PortRange { get; init; } = Core.Networking.PortRange.Default;

    public IOutputConsumer? OutputConsumer { get; init; }
}
