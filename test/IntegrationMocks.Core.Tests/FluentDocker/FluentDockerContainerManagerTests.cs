using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AutoFixture;
using Ductus.FluentDocker.Services;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.FluentDocker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Core.Resources;
using IntegrationMocks.Core.Tests.Fixtures;
using Xunit;

namespace IntegrationMocks.Core.Tests.FluentDocker;

public sealed class FluentDockerContainerManagerTests : IDisposable
{
    private readonly IStringRepository _containerNameRepository;
    private readonly int _internalPort;
    private readonly IPort _externalPort;
    private readonly IHostService _docker;

    public FluentDockerContainerManagerTests()
    {
        var fixture = new Fixture();
        _containerNameRepository = new DirectoryStringRepository(
            FluentDockerContainerManager.DefaultContainerNameRepositoryDirectoryPath);
        var portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _internalPort = fixture.Create<int>() % 100 + 30000;
        WaitUntilPortFree();
        _externalPort = portManager.TakePort(new Range<int>(UniquePorts.FluentDockerContainerManagerTests));
        var hosts = new Hosts().Discover();
        _docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default")
            ?? throw new SystemException("Could not connect to docker host.");
    }

    public void Dispose()
    {
        _docker.Dispose();
        _externalPort.Dispose();
    }

    [Fact]
    public async Task StartContainer_can_start_busybox_container()
    {
        var nameGenerator = new NameGenerator();
        var sut = new FluentDockerContainerManager(_containerNameRepository);

        using var container = await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);

        Assert.True(Ping(_externalPort.Number));
        Assert.True(ContainerExists(nameGenerator.LastGeneratedName!));
    }

    [Fact]
    public async Task StartContainer_marks_container_name_as_used_in_repository()
    {
        var nameGenerator = new NameGenerator();
        var sut = new FluentDockerContainerManager(_containerNameRepository);

        using var container = await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);

        Assert.Contains(nameGenerator.LastGeneratedName, _containerNameRepository.GetAll());
    }

    [Fact]
    public async Task StartContainer_creates_container_that_can_be_destroyed_with_dispose()
    {
        var nameGenerator = new NameGenerator();
        var sut = new FluentDockerContainerManager(_containerNameRepository);
        using var container = await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);

        container.Dispose();

        Assert.False(Ping(_externalPort.Number));
        Assert.False(ContainerExists(nameGenerator.LastGeneratedName!));
        Assert.DoesNotContain(nameGenerator.LastGeneratedName, _containerNameRepository.GetAll());
    }

    [Fact]
    public async Task StartContainer_creates_container_that_can_be_destroyed_with_finalizer()
    {
        var nameGenerator = new NameGenerator();
        var sut = new FluentDockerContainerManager(_containerNameRepository);

        await CreateContainerAndForget(sut, nameGenerator);
        GC.Collect(2);
        GC.WaitForPendingFinalizers();

        Assert.False(Ping(_externalPort.Number));
        Assert.False(ContainerExists(nameGenerator.LastGeneratedName!));
        Assert.DoesNotContain(nameGenerator.LastGeneratedName, _containerNameRepository.GetAll());
    }

    [Fact]
    public async Task StartContainer_throws_when_same_name_is_already_in_use()
    {
        var nameGenerator = new NameGenerator(RandomName.PrefixGuid(nameof(FluentDockerContainerManagerTests)));
        var sut = new FluentDockerContainerManager(_containerNameRepository);
        using var container = await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAllContainers_destroys_containers()
    {
        var nameGenerator = new NameGenerator();
        var sut = new FluentDockerContainerManager(_containerNameRepository);
        using var container = await sut.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);

        await sut.DeleteAllContainers(nameGenerator.Matches, CancellationToken.None);

        Assert.False(Ping(_externalPort.Number));
        Assert.False(ContainerExists(nameGenerator.LastGeneratedName!));
        Assert.DoesNotContain(nameGenerator.LastGeneratedName, _containerNameRepository.GetAll());
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private async Task CreateContainerAndForget(IDockerContainerManager manager, INameGenerator nameGenerator)
    {
        await manager.StartContainer(nameGenerator, ConfigureBusybox, CancellationToken.None);
    }

    private void ConfigureBusybox(IDockerContainerBuilder builder)
    {
        builder
            .UseImage("busybox")
            .WithEnvironment($"INTERNAL_PORT={_internalPort}")
            .ExposePort(_externalPort.Number, _internalPort)
            .Command("bin/sh", "-c", "\"nc -l $INTERNAL_PORT\"");
    }

    private bool ContainerExists(string containerName)
    {
        var containers = _docker.GetContainers(all: true, filters: $"name=^{Regex.Escape(containerName)}$");

        foreach (var container in containers)
        {
            container.Dispose();
        }

        return containers.Count > 0;
    }

    private static bool Ping(int port)
    {
        try
        {
            using (new TcpClient(IPAddress.Loopback.ToString(), port))
            {
                return true;
            }
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private static void WaitUntilPortFree()
    {
        var portMonitor = new PortMonitor();
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        TimeService.Instance.WaitUntil(
            () => portMonitor.GetUsedPorts(new Range<int>(UniquePorts.FluentDockerContainerManagerTests)).Count == 0,
            cancellation.Token);
    }

    private class NameGenerator : INameGenerator
    {
        private readonly string? _name;

        public NameGenerator(string? name = null)
        {
            _name = name;
        }

        public string? LastGeneratedName { get; private set; }

        public string GenerateName()
        {
            LastGeneratedName = _name ?? RandomName.PrefixGuid(nameof(FluentDockerContainerManagerTests));
            return LastGeneratedName;
        }

        public bool Matches(string containerName)
        {
            return containerName.StartsWith($"{nameof(FluentDockerContainerManagerTests)}_");
        }
    }
}
