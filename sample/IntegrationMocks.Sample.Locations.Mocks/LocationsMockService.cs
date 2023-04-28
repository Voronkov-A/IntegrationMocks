using IntegrationMocks.Core.Networking;
using IntegrationMocks.Sample.Locations.Mocks.Adapters.WebApi;
using IntegrationMocks.Web.Mocks;
using Moq;

namespace IntegrationMocks.Sample.Locations.Mocks;

public class LocationsMockService : DefaultMockService<LocationsMockContract>
{
    public LocationsMockService(IPortManager portManager) : base(portManager)
    {
        var locationsController = new Mock<LocationsControllerBase>()
        {
            CallBase = true
        };
        Contract = new LocationsMockContract(WebApiPort.Number, locationsController);
        AddController(locationsController.Object);
    }

    public override LocationsMockContract Contract { get; }
}
