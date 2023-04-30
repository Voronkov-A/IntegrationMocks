using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace IntegrationMocks.Core.FluentDocker;

public class FluentDockerContainerManager : IDockerContainerManager
{
    public static readonly string DefaultContainerNameRepositoryDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        $"{nameof(IntegrationMocks)}_{nameof(FluentDockerContainerManager)}");

    public static readonly FluentDockerContainerManager Default = new(
        new DirectoryStringRepository(DefaultContainerNameRepositoryDirectoryPath),
        NullLogger<FluentDockerContainerManager>.Instance);

    private readonly IStringRepository _containerNameRepository;
    private readonly ILogger<FluentDockerContainerManager> _logger;

    public FluentDockerContainerManager(
        IStringRepository containerNameRepository,
        ILogger<FluentDockerContainerManager> logger)
    {
        _containerNameRepository = containerNameRepository;
        _logger = logger;
    }

    public FluentDockerContainerManager(IStringRepository containerNameRepository)
        : this(containerNameRepository, NullLogger<FluentDockerContainerManager>.Instance)
    {
    }

    public FluentDockerContainerManager(ILogger<FluentDockerContainerManager> logger)
        : this(new DirectoryStringRepository(DefaultContainerNameRepositoryDirectoryPath), logger)
    {
    }

    public FluentDockerContainerManager()
        : this(
            new DirectoryStringRepository(DefaultContainerNameRepositoryDirectoryPath),
            NullLogger<FluentDockerContainerManager>.Instance)
    {
    }

    public ValueTask<IDockerContainer> StartContainer(
        INameGenerator containerNameGenerator,
        Action<IDockerContainerBuilder> configure,
        CancellationToken cancellationToken)
    {
        IDockerContainer container = new FluentDockerContainer(
            new DockerContainerHandle(this, containerNameGenerator, configure, _logger));

        if (cancellationToken.IsCancellationRequested)
        {
            container.Dispose();
            cancellationToken.ThrowIfCancellationRequested();
        }

        return ValueTask.FromResult(container);
    }

    public ValueTask DeleteAllContainers(Func<string, bool> containerNamePredicate, CancellationToken cancellationToken)
    {
        var usedContainerNames = _containerNameRepository.GetAll();

        var hosts = new Hosts().Discover();
        using var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (docker == null)
        {
            throw new SystemException("Could not connect to docker host.");
        }

        foreach (var containerName in usedContainerNames.Where(containerNamePredicate))
        {
            var containers = docker.GetContainers(all: true, filters: $"name=^{Regex.Escape(containerName)}$");

            foreach (var container in containers)
            {
                using (new DockerContainerHandle(this, containerName, container, _logger))
                {
                }
            }
        }

        return ValueTask.CompletedTask;
    }

    private string CreateContainerName(INameGenerator containerNameGenerator)
    {
        var usedContainerNames = _containerNameRepository.GetAll();
        var firstContainerName = containerNameGenerator.GenerateName();
        var containerName = firstContainerName;

        while (usedContainerNames.Contains(containerName) || !_containerNameRepository.Add(containerName))
        {
            containerName = containerNameGenerator.GenerateName();

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
        private readonly ILogger _logger;

        public DockerContainerHandle(
            FluentDockerContainerManager manager,
            INameGenerator containerNameGenerator,
            Action<IDockerContainerBuilder> configure,
            ILogger logger)
            : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            _containerName = _manager.CreateContainerName(containerNameGenerator);
            _logger = logger;

            var builder = new Builder().UseContainer();
            builder.RemoveVolumesOnDispose(true);
            var containerBuilder = new FluentDockerContainerBuilder(builder);
            configure(containerBuilder);
            builder.WithName(_containerName);
            ContainerService = builder.Build();
            _logger.LogDebug(
                "Starting container {@containerName} with external ports {@externalPorts}.",
                _containerName,
                containerBuilder.ExternalPorts);
            ContainerService.Start();
        }

        public DockerContainerHandle(
            FluentDockerContainerManager manager,
            string containerName,
            IContainerService containerService,
            ILogger logger)
            : base(IntPtr.Zero, true)
        {
            _processExitHook = WeakDisposeEventHandler.Create(this);
            AppDomain.CurrentDomain.ProcessExit += _processExitHook;

            _manager = manager;
            _containerName = containerName;
            ContainerService = containerService;
            _logger = logger;
            _logger.LogDebug("Attaching to container {@containerName}.", _containerName);
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
                _logger.LogDebug("Deleting container {@containerName}.", _containerName);
                _manager.DeleteContainerName(_containerName);
            }

            AppDomain.CurrentDomain.ProcessExit -= _processExitHook;

            return true;
        }
    }
}
