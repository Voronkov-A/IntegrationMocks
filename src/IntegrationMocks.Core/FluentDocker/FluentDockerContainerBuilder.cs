using Ductus.FluentDocker.Builders;
using IntegrationMocks.Core.Docker;

namespace IntegrationMocks.Core.FluentDocker;

public class FluentDockerContainerBuilder : IDockerContainerBuilder
{
    private readonly ContainerBuilder _builder;
    private readonly HashSet<int> _externalPorts;

    public FluentDockerContainerBuilder(ContainerBuilder builder)
    {
        _builder = builder;
        _externalPorts = new HashSet<int>();
    }

    public IReadOnlyCollection<int> ExternalPorts => _externalPorts;

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
        _externalPorts.Add(hostPort);
        return this;
    }

    public IDockerContainerBuilder Command(string command, params string[] arguments)
    {
        _builder.Command(command, arguments);
        return this;
    }
}
