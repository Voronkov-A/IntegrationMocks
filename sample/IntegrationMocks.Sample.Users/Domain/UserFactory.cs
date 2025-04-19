using System;

namespace IntegrationMocks.Sample.Users.Domain;

internal sealed class UserFactory : IUserFactory
{
    public User CreateUser(string name, Location location)
    {
        ArgumentNullException.ThrowIfNull(name);

        return new User(Guid.NewGuid(), name, location);
    }
}
