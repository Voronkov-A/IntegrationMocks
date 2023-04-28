using Ductus.FluentDocker.Services;

namespace IntegrationMocks.Core.FluentDocker;

public static class ContainerServiceExtensions
{
    public static void Destroy(this IContainerService containerService)
    {
        try
        {
            containerService.Remove(force: true);
        }
        catch
        {
            //
        }

        try
        {
            containerService.Dispose();
        }
        catch
        {
            //
        }
    }
}
