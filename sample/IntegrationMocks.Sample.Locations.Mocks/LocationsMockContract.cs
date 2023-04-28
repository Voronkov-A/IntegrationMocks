using IntegrationMocks.Sample.Locations.Mocks.Adapters.WebApi;
using Moq;

namespace IntegrationMocks.Sample.Locations.Mocks;

public record LocationsMockContract(int WebApiPort, Mock<LocationsControllerBase> LocationsController);
