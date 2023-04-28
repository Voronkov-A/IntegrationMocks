using AutoFixture;
using IntegrationMocks.Sample.Locations.Domain;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures.Customizations;

public class LocationCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Location>(composer => composer.FromFactory(() => new Location(
            id: fixture.Create<Guid>(),
            name: fixture.Create<string>())));
    }
}
