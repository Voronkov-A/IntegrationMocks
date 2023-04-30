using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;

namespace IntegrationMocks.Core.FluentDocker;

public static class ContainerServiceExtensions
{
    public static void Destroy(this IContainerService containerService)
    {
        try
        {
            containerService.Stop();
            containerService.WaitForStopped();
        }
        catch
        {
            //
        }

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
