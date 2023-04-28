namespace IntegrationMocks.Core.Docker;

public interface IDockerContainerManager
{
    ValueTask<IDockerContainer> StartContainer(
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken);

    ValueTask DeleteAllContainers(CancellationToken cancellationToken);
}
