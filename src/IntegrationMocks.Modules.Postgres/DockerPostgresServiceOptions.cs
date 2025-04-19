using DotNet.Testcontainers.Configurations;
using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Modules.Postgres;

public sealed class DockerPostgresServiceOptions
{
    public string Username { get; init; } = "postgres";

    public string Password { get; init; } = "postgres";

    public string Image { get; init; } = "postgres:17.4";

    public Range<int> PortRange { get; init; } = Core.Networking.PortRange.Default;

    public IOutputConsumer? OutputConsumer { get; init; }
}
