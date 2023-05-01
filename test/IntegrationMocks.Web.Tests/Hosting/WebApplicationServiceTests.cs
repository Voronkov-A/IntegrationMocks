using System.Net.Sockets;
using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Web.Hosting;
using IntegrationMocks.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationMocks.Web.Tests.Hosting;

public class WebApplicationServiceTests
{
    private readonly IPortManager _portManager;
    private readonly string _healthPath;

    public WebApplicationServiceTests()
    {
        var fixture = new Fixture();
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
        _healthPath = $"/{fixture.Create<string>()}";
    }

    [Fact]
    public async Task Constructor_does_not_start_host()
    {
        await using var sut = new TestWebApplicationService(_portManager, _healthPath);

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_host_available()
    {
        await using var sut = new TestWebApplicationService(_portManager, _healthPath);

        await sut.InitializeAsync();

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new TestWebApplicationService(_portManager, _healthPath);

        await sut.InitializeAsync();
        var firstPort = sut.Contract.WebApiPort;
        await sut.InitializeAsync();
        var secondPort = sut.Contract.WebApiPort;

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.True(ping);
        Assert.Equal(firstPort, secondPort);
    }

    [Fact]
    public async Task DisposeAsync_makes_host_unavailable()
    {
        await using var sut = new TestWebApplicationService(
            _portManager,
            _healthPath,
            new Range<int>(UniquePorts.WebApplicationServiceTests));
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.False(ping);
    }

    private async Task<bool> Ping(int port)
    {
        using var client = new HttpClient()
        {
            BaseAddress = UriUtils.HttpLocalhost(port)
        };

        try
        {
            var status = await client.GetStringAsync(_healthPath);
            return status == "Healthy";
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException)
        {
            return false;
        }
    }

    private class TestWebApplicationService : WebApplicationService<DefaultHostServiceContract>
    {
        private readonly IPort _webApiPort;
        private readonly string _healthPath;

        public TestWebApplicationService(IPortManager portManager, string healthPath, Range<int> portRange)
        {
            _webApiPort = portManager.TakePort(portRange);
            _healthPath = healthPath;
            Contract = new DefaultHostServiceContract(_webApiPort.Number);
        }

        public TestWebApplicationService(IPortManager portManager, string healthPath)
            : this(portManager, healthPath, PortRange.Default)
        {
        }

        public override DefaultHostServiceContract Contract { get; }

        protected override WebApplicationBuilder CreateWebApplicationBuilder()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddHealthChecks();
            builder.WebHost.UseKestrel(opt => opt.ListenAnyIP(Contract.WebApiPort));
            return builder;
        }

        protected override void Configure(WebApplication app)
        {
            app.UseHealthChecks(_healthPath);
        }

        protected override async ValueTask DisposeAsync(bool disposing)
        {
            await base.DisposeAsync(disposing);

            if (disposing)
            {
                _webApiPort.Dispose();
            }
        }
    }
}
