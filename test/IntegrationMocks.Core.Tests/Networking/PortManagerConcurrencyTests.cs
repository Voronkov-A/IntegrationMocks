using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.AspNetCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Core.Tests.Networking;

/// <summary>
/// Run 3 instances of
/// dotnet test --filter "FullyQualifiedName=IntegrationMocks.Core.Tests.Networking.PortManagerConcurrencyTests.TakePort_Is_Safe" --no-build
/// in parallel
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage",
    "xUnit1004:Test methods should not be skipped")]
public sealed class PortManagerConcurrencyTests
{
    private readonly SemaphoreSlim _limiter = new(300);

    [Fact(Skip = "Takes too long.")]
    public async Task TakePort_Is_Safe()
    {
        var portManager = PortManager.Default;
        var portRange = PortRange.Default;
        using var cancellation = new CancellationTokenSource();
        await Task.WhenAll(Enumerable
            .Range(0, 99999)
            .Select(_ => RunService(portManager, portRange, cancellation)));
    }

    private async Task RunService(
        IPortManager portManager,
        Range<int> portRange,
        CancellationTokenSource cancellation)
    {
        if (cancellation.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await _limiter.WaitAsync(cancellation.Token);
            try
            {
                await using var service = new MockedTestServer(portManager, portRange);
                await service.InitializeAsync(cancellation.Token);
                await Task.Delay(new Random().Next(0, 5000), cancellation.Token);
            }
            finally
            {
                _limiter.Release();
            }
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cancellation.Token)
        {
            // pass
        }
        catch
        {
            cancellation.Cancel();
            throw;
        }
    }

    private class TestServerContract
    {
    }

    private class MockedTestServer : MockWebApplicationService<TestServerContract>
    {
        public MockedTestServer(IPortManager portManager, Range<int> portRange)
            : base(portManager, portRange)
        {
            Contract = new TestServerContract();
        }

        public override TestServerContract Contract { get; }
    }
}
