namespace IntegrationMocks.Sample.Locations.Domain;

internal interface ILocationFactory
{
    Location CreateLocation(string name);
}
