using Ductus.FluentDocker.Services;
using IntegrationMocks.Core.Docker;

namespace IntegrationMocks.Core.FluentDocker;

public static class ServiceRunningStateExtensions
{
    public static DockerContainerState ToDockerContainerState(this ServiceRunningState state)
    {
        return state switch
        {
            ServiceRunningState.Unknown => DockerContainerState.Unknown,
            ServiceRunningState.Starting => DockerContainerState.Starting,
            ServiceRunningState.Running => DockerContainerState.Running,
            ServiceRunningState.Paused => DockerContainerState.Paused,
            ServiceRunningState.Stopping => DockerContainerState.Stopping,
            ServiceRunningState.Stopped => DockerContainerState.Stopped,
            ServiceRunningState.Removing => DockerContainerState.Removing,
            ServiceRunningState.Removed => DockerContainerState.Removed,
            _ => throw new NotSupportedException($"{nameof(ServiceRunningState)}.{state} is not supported.")
        };
    }
}
