using IntegrationMocks.Core;

namespace IntegrationMocks.Library.Sql;

public class EnvironmentSqlService : ExternalInfrastructureService<SqlServiceContract>
{
    public EnvironmentSqlService() : base(CreateContract())
    {
    }

    private static SqlServiceContract CreateContract()
    {
        return new SqlServiceContract(
            username: Environment.GetEnvironmentVariable("SqlServiceContract_Username") ?? "postgres",
            password: Environment.GetEnvironmentVariable("SqlServiceContract_Password") ?? "postgres",
            host: Environment.GetEnvironmentVariable("SqlServiceContract_Host") ?? "localhost",
            port: int.Parse(Environment.GetEnvironmentVariable("SqlServiceContract_Port") ?? "5432"));
    }
}
