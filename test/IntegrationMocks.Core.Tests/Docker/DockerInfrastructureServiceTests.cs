using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.Resources;
using Moq;
using Xunit;

namespace IntegrationMocks.Core.Tests.Docker;

public class DockerInfrastructureServiceTests
{
    private readonly Mock<IDockerContainerManager> _dockerContainerManagerMock;
    private readonly IDockerContainer _container;
    private readonly TestDockerInfrastructureService _sut;

    public DockerInfrastructureServiceTests()
    {
        _dockerContainerManagerMock = new Mock<IDockerContainerManager>();
        _container = Mock.Of<IDockerContainer>();
        _dockerContainerManagerMock
            .Setup(x => x.StartContainer(
                It.IsAny<INameGenerator>(),
                It.IsAny<Action<IDockerContainerBuilder>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_container);
        _sut = new TestDockerInfrastructureService(_dockerContainerManagerMock.Object);
    }

    [Fact]
    public async Task InitializeAsync_starts_docker_container()
    {
        await _sut.InitializeAsync();

        _dockerContainerManagerMock.Verify(
            x => x.StartContainer(
                It.IsAny<INameGenerator>(),
                _sut.ConfigureContainerAction,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_waits_until_container_is_ready()
    {
        await _sut.InitializeAsync();

        Assert.Equal(1, _sut.WaitUntilReadyInvocationCount);
        Assert.Equal(_sut.Container, _container);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await _sut.InitializeAsync();
        await _sut.InitializeAsync();

        _dockerContainerManagerMock.Verify(
            x => x.StartContainer(
                It.IsAny<INameGenerator>(),
                _sut.ConfigureContainerAction,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private class TestDockerInfrastructureService : DockerInfrastructureService<object>
    {
        public TestDockerInfrastructureService(IDockerContainerManager containerManager)
            : base(containerManager, new RandomNameGenerator(nameof(TestDockerInfrastructureService)))
        {
            Contract = new object();
        }

        public override object Contract { get; }

        public Action<IDockerContainerBuilder> ConfigureContainerAction => ConfigureContainer;

        public int WaitUntilReadyInvocationCount { get; private set; }

        public IDockerContainer? Container { get; private set; }

        protected override void ConfigureContainer(IDockerContainerBuilder builder)
        {
            // pass
        }

        protected override Task WaitUntilReady(IDockerContainer container, CancellationToken cancellationToken)
        {
            ++WaitUntilReadyInvocationCount;
            Container = container;
            return Task.CompletedTask;
        }
    }
}
