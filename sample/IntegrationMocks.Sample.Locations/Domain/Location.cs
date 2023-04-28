namespace IntegrationMocks.Sample.Locations.Domain;

public class Location
{
    internal Location(Guid id, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        Name = name;
    }

    public Guid Id { get; }

    public string Name { get; }
}
