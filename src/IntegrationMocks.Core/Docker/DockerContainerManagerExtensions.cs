using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.Docker;

public static class DockerContainerManagerExtensions
{
    private static readonly INameGenerator ContainerNameGenerator = new RandomNameGenerator(
        $"{nameof(IntegrationMocks)}_{nameof(DockerContainerManagerExtensions)}");

    public static async ValueTask<IDockerContainer> StartContainer(
        this IDockerContainerManager self,
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        return await self.StartContainer(ContainerNameGenerator, configure, cancellationToken);
    }

    public static async ValueTask<IDockerContainer> StartContainer(
        this IDockerContainerManager self,
        string containerNamePrefix,
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        var containerNameGenerator = new RandomNameGenerator(containerNamePrefix);
        return await self.StartContainer(containerNameGenerator, configure, cancellationToken);
    }

    public static async ValueTask DeleteAllContainers(
        this IDockerContainerManager self,
        CancellationToken cancellationToken = default)
    {
        await self.DeleteAllContainers(_ => true, cancellationToken);
    }
}
