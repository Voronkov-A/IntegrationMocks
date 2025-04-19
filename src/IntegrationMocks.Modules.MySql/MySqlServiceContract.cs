namespace IntegrationMocks.Modules.MySql;

public sealed class MySqlServiceContract
{
    public required string Username { get; init; }

    public required string Password { get; init;  }

    public required string Host { get; init; }

    public required int Port { get; init; }
}
