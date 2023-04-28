namespace IntegrationMocks.Library.Sql;

public class DockerPostgresServiceOptions
{
    public string Username { get; init; } = "postgres";

    public string Password { get; init; } = "postgres";

    public string Image { get; init; } = "postgres:latest";
}
