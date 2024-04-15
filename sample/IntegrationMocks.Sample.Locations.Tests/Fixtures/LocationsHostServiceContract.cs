namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

public class LocationsHostServiceContract
{
    public LocationsHostServiceContract(int webApiPort)
    {
        WebApiPort = webApiPort;
    }

    public int WebApiPort { get; }
}
