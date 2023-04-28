using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.FluentDocker;

public class FluentDockerContainerManager : IDockerContainerManager
{
    public static readonly string DefaultContainerNameRepositoryDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        $"{nameof(IntegrationMocks)}_{nameof(FluentDockerContainerManager)}");

    public static readonly FluentDockerContainerManager Default = new(
        new RandomNameGenerator($"{nameof(IntegrationMocks)}_{nameof(FluentDockerContainerManager)}"),
        new DirectoryStringRepository(DefaultContainerNameRepositoryDirectoryPath));

    private readonly INameGenerator _containerNameGenerator;
    private readonly IStringRepository _containerNameRepository;

    public FluentDockerContainerManager(
        INameGenerator containerNameGenerator,
        IStringRepository containerNameRepository)
    {
        _containerNameGenerator = containerNameGenerator;
        _containerNameRepository = containerNameRepository;
    }

    public ValueTask<IDockerContainer> StartContainer(
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken)
    {
        IDockerContainer container = new FluentDockerContainer(new DockerContainerHandle(this, configure));

        if (cancellationToken.IsCancellationRequested)
        {
            container.Dispose();
            cancellationToken.ThrowIfCancellationRequested();
        }

        return ValueTask.FromResult(container);
    }

    public ValueTask DeleteAllContainers(CancellationToken cancellationToken)
    {
        var usedContainerNames = _containerNameRepository.GetAll();

        var hosts = new Hosts().Discover();
        using var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (docker == null)
        {
            throw new SystemException("Could not connect to docker host.");
        }

        foreach (var containerName in usedContainerNames)
        {
            var containers = docker.GetContainers(all: true, filters: $"name=^{Regex.Escape(containerName)}$");

            foreach (var container in containers)
            {
                using (new DockerContainerHandle(this, containerName, container))
                {
                }
            }
        }

        return ValueTask.CompletedTask;
    }

    private string CreateContainerName()
    {
        var usedContainerNames = _containerNameRepository.GetAll();
        var firstContainerName = _containerNameGenerator.GenerateName();
        var containerName = firstContainerName;

        while (usedContainerNames.Contains(containerName) || !_containerNameRepository.Add(containerName))
        {
            containerName = _containerNameGenerator.GenerateName();

            if (containerName == firstContainerName)
            {
                throw new InvalidOperationException($"Could not create container with name {containerName}.");
            }
        }

        return containerName;
    }

    private void DeleteContainerName(string containerName)
    {
        _containerNameRepository.Remove(containerName);
    }

    private class DockerContainerHandle : SafeHandle, IFluentDockerContainerHandle
    {
        private readonly FluentDockerContainerManager _manager;
        private readonly string _containerName;
        private readonly EventHandler _processExitHook;

        public DockerContainerHandle(FluentDockerContainerManager manager, Action<IDockerContainerBuilder> configure)
            : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            _containerName = _manager.CreateContainerName();

            var builder = new Builder().UseContainer();
            builder.RemoveVolumesOnDispose(true);
            var containerBuilder = new FluentDockerContainerBuilder(builder);
            configure(containerBuilder);
            builder.WithName(_containerName);
            ContainerService = builder.Build();
            ContainerService.Start();
        }

        public DockerContainerHandle(
            FluentDockerContainerManager manager,
            string containerName,
            IContainerService containerService)
            : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            _containerName = containerName;
            ContainerService = containerService;
        }

        public IContainerService ContainerService { get; }

        public override bool IsInvalid => false;

        [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        protected override bool ReleaseHandle()
        {
            ContainerService?.Destroy();

            if (_containerName != null)
            {
                _manager.DeleteContainerName(_containerName);
            }

            AppDomain.CurrentDomain.ProcessExit -= _processExitHook;

            return true;
        }
    }
}
