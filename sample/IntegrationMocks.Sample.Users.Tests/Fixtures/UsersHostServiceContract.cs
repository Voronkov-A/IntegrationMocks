namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public class UsersHostServiceContract
{
    public UsersHostServiceContract(int webApiPort)
    {
        WebApiPort = webApiPort;
    }

    public int WebApiPort { get; }
}
