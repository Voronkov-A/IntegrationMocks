using System;
using IntegrationMocks.Sample.Locations.Mocks.Adapters.WebApi;
using Moq;

namespace IntegrationMocks.Sample.Locations.Mocks;

public sealed class LocationsMockContract
{
    public required Uri WebApiUrl { get; init; }

    public required Mock<LocationsControllerBase> LocationsController { get; init; }
}

