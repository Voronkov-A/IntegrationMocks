namespace IntegrationMocks.Web.Hosting;

public class DefaultHostServiceContract
{
    public DefaultHostServiceContract(int webApiPort)
    {
        WebApiPort = webApiPort;
    }

    public int WebApiPort { get; }
}
