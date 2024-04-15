using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationMocks.Modules.AspNetCore;

internal static class ServiceCollectionExtensions
{
    public static IMvcBuilder AddExplicitControllers(
        this IServiceCollection services,
        IEnumerable<IControllerRegistrar> controllerRegistrars)
    {
        var controllerRegistrarList
            = controllerRegistrars as IReadOnlyCollection<IControllerRegistrar>
            ?? controllerRegistrars.ToList();

        var mvcBuilder = services
            .AddControllers()
            .AddControllersAsServices()
            .ConfigureApplicationPartManager(x =>
            {
                var controllerFeatureProviders = x.FeatureProviders
                    .Where(provider => provider is IApplicationFeatureProvider<ControllerFeature>)
                    .ToList();

                foreach (var featureProvider in controllerFeatureProviders)
                {
                    x.FeatureProviders.Remove(featureProvider);
                }

                var controllerTypes = controllerRegistrarList
                    .Select(controller => controller.ControllerType);
                x.FeatureProviders.Add(new ExplicitControllerFeatureProvider(controllerTypes));
            });

        foreach (var controllerRegistrar in controllerRegistrarList)
        {
            controllerRegistrar.Register(services);
        }

        return mvcBuilder;
    }
}
