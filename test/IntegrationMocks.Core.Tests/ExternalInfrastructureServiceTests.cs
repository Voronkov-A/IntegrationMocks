using Xunit;

namespace IntegrationMocks.Core.Tests;

public class ExternalInfrastructureServiceTests
{
    [Fact]
    public void Contract_is_passed_as_is()
    {
        var expectedContract = new object();

        using var sut = new ExternalInfrastructureService<object>(expectedContract);
        var actualContract = sut.Contract;

        Assert.Same(expectedContract, actualContract);
    }
}
