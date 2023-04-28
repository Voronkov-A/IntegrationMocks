using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IntegrationMocks.Web.Mocks;

public class ExplicitControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly List<Type> _controllerTypes;

    public ExplicitControllerFeatureProvider(IEnumerable<Type> controllerTypes)
    {
        _controllerTypes = controllerTypes.ToList();
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        feature.Controllers.Clear();

        foreach (var controllerType in _controllerTypes)
        {
            feature.Controllers.Add(controllerType.GetTypeInfo());
        }
    }
}
