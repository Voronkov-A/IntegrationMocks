using IntegrationMocks.Core;
using IntegrationMocks.Modules.Sql;
using System;

namespace IntegrationMocks.Modules.Postgres;

public class EnvironmentPostgresService : ExternalInfrastructureService<SqlServiceContract>
{
    public EnvironmentPostgresService() : base(CreateContract())
    {
    }

    private static SqlServiceContract CreateContract()
    {
        return new SqlServiceContract(
            username: Environment.GetEnvironmentVariable("SqlServiceContract_Username")
                ?? "postgres",
            password: Environment.GetEnvironmentVariable("SqlServiceContract_Password")
                ?? "postgres",
            host: Environment.GetEnvironmentVariable("SqlServiceContract_Host")
                ?? "localhost",
            port: int.Parse(Environment.GetEnvironmentVariable("SqlServiceContract_Port")
                ?? "5432"));
    }
}
