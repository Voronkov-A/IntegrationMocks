namespace IntegrationMocks.Core.Docker;

public interface IDockerContainerBuilder
{
    IDockerContainerBuilder UseImage(string image);

    IDockerContainerBuilder WithEnvironment(params string[] nameValue);

    IDockerContainerBuilder ExposePort(int hostPort, int containerPort);

    IDockerContainerBuilder Command(string command, params string[] arguments);
}
