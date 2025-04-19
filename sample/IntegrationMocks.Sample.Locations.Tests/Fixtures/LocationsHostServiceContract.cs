using System;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

internal sealed class LocationsHostServiceContract
{
    public required Uri WebApiUrl { get; init; }
}
