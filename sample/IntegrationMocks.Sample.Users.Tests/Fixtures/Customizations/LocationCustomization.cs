using AutoFixture;
using IntegrationMocks.Sample.Users.Domain;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures.Customizations;

public class LocationCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Location>(composer => composer.FromFactory(() => new Location(id: fixture.Create<Guid>())));
    }
}
