using Ductus.FluentDocker.Services;

namespace IntegrationMocks.Core.FluentDocker;

public interface IFluentDockerContainerHandle : IDisposable
{
    IContainerService ContainerService { get; }
}
