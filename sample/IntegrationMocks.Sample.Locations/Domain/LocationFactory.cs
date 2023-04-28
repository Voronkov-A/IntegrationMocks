namespace IntegrationMocks.Sample.Locations.Domain;

public class LocationFactory : ILocationFactory
{
    public Location CreateLocation(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return new Location(Guid.NewGuid(), name);
    }
}
