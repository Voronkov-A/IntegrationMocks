namespace IntegrationMocks.Modules.Postgres;

public sealed class PostgresServiceContract
{
    public required string Username { get; init; }

    public required string Password { get; init;  }

    public required string Host { get; init; }

    public required int Port { get; init; }
}
