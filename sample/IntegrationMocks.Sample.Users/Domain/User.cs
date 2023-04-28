namespace IntegrationMocks.Sample.Users.Domain;

public class User
{
    internal User(Guid id, string name, Location location)
    {
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        Name = name;
        Location = location;
    }

    public Guid Id { get; }

    public string Name { get; }

    public Location Location { get; }
}
