using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Web.Hosting;
using IntegrationMocks.Web.Mocks;
using IntegrationMocks.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace IntegrationMocks.Web.Tests.Mocks;

public class DefaultMockServiceTests
{
    private readonly IFixture _fixture;
    private readonly IPortManager _portManager;
    private readonly string _value;

    public DefaultMockServiceTests()
    {
        _fixture = new Fixture();
        _portManager = PortManager.Default;
        _value = _fixture.Create<string>();
    }

    [Fact]
    public async Task Constructor_does_not_start_host()
    {
        await using var sut = new TestMockService(_portManager, _value);

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.False(ping);
    }

    [Fact]
    public async Task InitializeAsync_makes_host_available()
    {
        await using var sut = new TestMockService(_portManager, _value);

        await sut.InitializeAsync();

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.True(ping);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        await using var sut = new TestMockService(_portManager, _value);

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
        await using var sut = new TestMockService(
            _portManager,
            _value,
            new Range<int>(UniquePorts.DefaultMockServiceTests));
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        var ping = await Ping(sut.Contract.WebApiPort);
        Assert.False(ping);
    }

    [Fact]
    public async Task GetValue_returns_initial_value()
    {
        await using var sut = new TestMockService(_portManager, _value);
        await sut.InitializeAsync();

        var actualValue = await GetValue(sut.Contract.WebApiPort);

        Assert.Equal(_value, actualValue);
    }

    [Fact]
    public async Task GetValue_returns_configured_value()
    {
        var expectedValue = _fixture.Create<string>();
        await using var sut = new TestMockService(_portManager, _value);
        await sut.InitializeAsync();
        sut.Contract.Controller.Value = expectedValue;

        var actualValue = await GetValue(sut.Contract.WebApiPort);

        Assert.Equal(expectedValue, actualValue);
    }

    private static async Task<string?> GetValue(int port)
    {
        using var client = new HttpClient
        {
            BaseAddress = UriUtils.HttpLocalhost(port)
        };

        return await client.GetStringAsync(TestController.ValuePath);
    }

    private static async Task<bool> Ping(int port)
    {
        using var client = new HttpClient
        {
            BaseAddress = UriUtils.HttpLocalhost(port)
        };

        try
        {
            var response = await client.GetAsync(TestController.ValuePath);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException)
        {
            return false;
        }
    }

    private class TestController
    {
        public const string ValuePath = "/value";

        public TestController(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        [HttpGet(ValuePath)]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public ValueTask<string> GetValue()
        {
            return ValueTask.FromResult(Value);
        }
    }

    private class TestMockContract : DefaultHostServiceContract
    {
        public TestMockContract(int webApiPort, TestController controller) : base(webApiPort)
        {
            Controller = controller;
        }

        public TestController Controller { get; }
    }

    private class TestMockService : DefaultMockService<TestMockContract>
    {
        public TestMockService(IPortManager portManager, string value, Range<int> portRange)
            : base(portManager, portRange)
        {
            var controller = new TestController(value);
            Contract = new TestMockContract(WebApiPort.Number, controller);
            AddController(controller);
        }

        public TestMockService(IPortManager portManager, string value) : this(portManager, value, PortRange.Default)
        {
        }

        public override TestMockContract Contract { get; }
    }
}
