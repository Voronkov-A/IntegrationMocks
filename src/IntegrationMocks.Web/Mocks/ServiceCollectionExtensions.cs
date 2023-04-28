using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationMocks.Web.Mocks;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplicitControllers(
        this IServiceCollection services,
        IEnumerable<IControllerRegistrar> controllerRegistrars)
    {
        var controllerRegistrarList = controllerRegistrars as IReadOnlyCollection<IControllerRegistrar>
                                      ?? controllerRegistrars.ToList();

        services.AddControllers().AddControllersAsServices().ConfigureApplicationPartManager(x =>
        {
            var controllerFeatureProviders = x.FeatureProviders
                .Where(provider => provider is IApplicationFeatureProvider<ControllerFeature>)
                .ToList();

            foreach (var featureProvider in controllerFeatureProviders)
            {
                x.FeatureProviders.Remove(featureProvider);
            }

            var controllerTypes = controllerRegistrarList.Select(controller => controller.ControllerType);
            x.FeatureProviders.Add(new ExplicitControllerFeatureProvider(controllerTypes));
        });

        foreach (var controllerRegistrar in controllerRegistrarList)
        {
            controllerRegistrar.Register(services);
        }

        return services;
    }
}
