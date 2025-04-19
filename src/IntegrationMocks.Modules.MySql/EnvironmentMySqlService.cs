using IntegrationMocks.Core;

namespace IntegrationMocks.Modules.MySql;

public sealed class EnvironmentMySqlService : ExternalInfrastructureService<MySqlServiceContract>
{
    public EnvironmentMySqlService() : this(new MySqlEnvironmentVariables())
    {
    }

    public EnvironmentMySqlService(MySqlEnvironmentVariables variables) : base(new MySqlServiceContract
    {
        Username = variables.Username.GetValue(),
        Password = variables.Password.GetValue(),
        Host = variables.Host.GetValue(),
        Port = int.Parse(variables.Port.GetValue())
    })
    {
    }
}
