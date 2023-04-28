namespace IntegrationMocks.Sample.Users.Domain;

public readonly struct Location : IEquatable<Location>
{
    internal Location(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public bool Equals(Location other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Location other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Location left, Location right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Location left, Location right)
    {
        return !(left == right);
    }
}
