using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntegrationMocks.Modules.AspNetCore;

internal class ExplicitControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
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
