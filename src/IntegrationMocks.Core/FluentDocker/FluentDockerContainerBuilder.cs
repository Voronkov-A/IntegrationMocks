using Ductus.FluentDocker.Builders;
using IntegrationMocks.Core.Docker;

namespace IntegrationMocks.Core.FluentDocker;

public class FluentDockerContainerBuilder : IDockerContainerBuilder
{
    private readonly ContainerBuilder _builder;

    public FluentDockerContainerBuilder(ContainerBuilder builder)
    {
        _builder = builder;
    }

    public IDockerContainerBuilder UseImage(string image)
    {
        _builder.UseImage(image);
        return this;
    }

    public IDockerContainerBuilder WithEnvironment(params string[] nameValue)
    {
        _builder.WithEnvironment(nameValue);
        return this;
    }

    public IDockerContainerBuilder ExposePort(int hostPort, int containerPort)
    {
        _builder.ExposePort(hostPort, containerPort);
        return this;
    }

    public IDockerContainerBuilder Command(string command, params string[] arguments)
    {
        _builder.Command(command, arguments);
        return this;
    }
}
