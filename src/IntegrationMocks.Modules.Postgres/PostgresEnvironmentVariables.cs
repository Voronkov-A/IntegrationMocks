using IntegrationMocks.Core.Environments;

namespace IntegrationMocks.Modules.Postgres;

public sealed class PostgresEnvironmentVariables
{
    public EnvironmentVariable Username { get; init; } = new("Postgres_Username", "postgres");

    public EnvironmentVariable Password { get; init; } = new("Postgres_Password", "postgres");

    public EnvironmentVariable Host { get; init; } = new("Postgres_Host", "localhost");

    public EnvironmentVariable Port { get; init; } = new("Postgres_Port", "5432");
}
