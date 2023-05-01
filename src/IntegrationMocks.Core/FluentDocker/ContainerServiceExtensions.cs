using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Logging;

namespace IntegrationMocks.Core.FluentDocker;

public static class ContainerServiceExtensions
{
    public static void Destroy(this IContainerService containerService, ILogger logger)
    {
        try
        {
            containerService.Remove(force: true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Could not remove container with name {@containerName}.",
                GetContainerNameOrNull(containerService));
        }

        try
        {
            containerService.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Could not dispose container with name {@containerName}.",
                GetContainerNameOrNull(containerService));
        }
    }

    private static string? GetContainerNameOrNull(IContainerService containerService)
    {
        try
        {
            return containerService.Name;
        }
        catch
        {
            return null;
        }
    }
}
