using IntegrationMocks.Core;

namespace IntegrationMocks.Modules.Postgres;

public sealed class EnvironmentPostgresService : ExternalInfrastructureService<PostgresServiceContract>
{
    public EnvironmentPostgresService() : this(new PostgresEnvironmentVariables())
    {
    }

    public EnvironmentPostgresService(PostgresEnvironmentVariables variables) : base(new PostgresServiceContract
    {
        Username = variables.Username.GetValue(),
        Password = variables.Password.GetValue(),
        Host = variables.Host.GetValue(),
        Port = int.Parse(variables.Port.GetValue())
    })
    {
    }
}
