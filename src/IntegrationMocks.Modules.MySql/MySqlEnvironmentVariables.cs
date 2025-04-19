using IntegrationMocks.Core.Environments;

namespace IntegrationMocks.Modules.MySql;

public sealed class MySqlEnvironmentVariables
{
    public EnvironmentVariable Username { get; init; } = new("MySql_Username", "root");

    public EnvironmentVariable Password { get; init; } = new("MySql_Password", "");

    public EnvironmentVariable Host { get; init; } = new("MySql_Host", "localhost");

    public EnvironmentVariable Port { get; init; } = new("MySql_Port", "3306");
}
