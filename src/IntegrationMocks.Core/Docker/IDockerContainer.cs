using IntegrationMocks.Core.Execution;

namespace IntegrationMocks.Core.Docker;

public interface IDockerContainer : IDisposable, IProcessStarter
{
    DockerContainerState State { get; }

    List<ProcessDescriptor> GetAllProcesses();
}
