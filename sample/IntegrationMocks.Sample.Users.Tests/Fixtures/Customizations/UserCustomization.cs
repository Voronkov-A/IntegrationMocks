using AutoFixture;
using IntegrationMocks.Sample.Users.Domain;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures.Customizations;

public class UserCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<User>(composer => composer.FromFactory(() => new User(
            id: fixture.Create<Guid>(),
            name: fixture.Create<string>(),
            location: fixture.Create<Location>())));
    }
}
