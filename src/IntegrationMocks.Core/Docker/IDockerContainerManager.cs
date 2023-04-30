using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.Docker;

public interface IDockerContainerManager
{
    ValueTask<IDockerContainer> StartContainer(
        INameGenerator containerNameGenerator,
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken);

    ValueTask DeleteAllContainers(Func<string, bool> containerNamePredicate, CancellationToken cancellationToken);
}
