using IntegrationMocks.Core;

namespace IntegrationMocks.Modules.Yugabyte;

public sealed class EnvironmentYugabyteService : ExternalInfrastructureService<YugabyteServiceContract>
{
    public EnvironmentYugabyteService() : this(new YugabyteEnvironmentVariables())
    {
    }

    public EnvironmentYugabyteService(YugabyteEnvironmentVariables variables) : base(new YugabyteServiceContract
    {
        Username = variables.Username.GetValue(),
        Password = variables.Password.GetValue(),
        Host = variables.Host.GetValue(),
        Port = int.Parse(variables.Port.GetValue())
    })
    {
    }
}
