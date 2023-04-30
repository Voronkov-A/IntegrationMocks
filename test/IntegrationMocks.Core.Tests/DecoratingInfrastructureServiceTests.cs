using Moq;
using Xunit;

namespace IntegrationMocks.Core.Tests;

public sealed class DecoratingInfrastructureServiceTests : IDisposable
{
    private readonly Mock<IInfrastructureService<object>> _innerMock;
    private readonly DecoratingInfrastructureService<object> _sut;

    public DecoratingInfrastructureServiceTests()
    {
        _innerMock = new Mock<IInfrastructureService<object>>();
        _sut = new DecoratingInfrastructureService<object>(_innerMock.Object);
    }

    public void Dispose()
    {
        _sut.Dispose();
    }

    [Fact]
    public void Contract_delegates_to_inner()
    {
        var actualContract = _sut.Contract;

        _innerMock.VerifyGet(x => x.Contract, Times.Once);
        Assert.Same(_innerMock.Object.Contract, actualContract);
    }

    [Fact]
    public async Task InitializeAsync_delegates_to_inner()
    {
        await _sut.InitializeAsync();

        _innerMock.Verify(x => x.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DisposeAsync_delegates_to_inner()
    {
        await _sut.DisposeAsync();

        _innerMock.Verify(x => x.DisposeAsync(), Times.Once);
    }
}
