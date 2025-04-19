using IntegrationMocks.Core.Environments;

namespace IntegrationMocks.Modules.Yugabyte;

public sealed class YugabyteEnvironmentVariables
{
    public EnvironmentVariable Username { get; init; } = new("Yugabyte_Username", "yugabyte");

    public EnvironmentVariable Password { get; init; } = new("Yugabyte_Password", "yugabyte");

    public EnvironmentVariable Host { get; init; } = new("Yugabyte_Host", "localhost");

    public EnvironmentVariable Port { get; init; } = new("Yugabyte_Port", "5432");
}
