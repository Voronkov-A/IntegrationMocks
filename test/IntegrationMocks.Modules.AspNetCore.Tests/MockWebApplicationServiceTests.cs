using AutoFixture;
using IntegrationMocks.Core;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Networking;
using IntegrationMocks.Modules.AspNetCore.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Modules.AspNetCore.Tests;

public class MockWebApplicationServiceTests
{
    private readonly IFixture _fixture;
    private readonly IPortManager _portManager;
    private readonly string _value;

    public MockWebApplicationServiceTests()
    {
        _fixture = new Fixture();
        _portManager = new PortManager(LoggerFixture.CreateLogger<PortManager>());
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
            BaseAddress = new Uri($"http://localhost:{port}")
        };

        return await client.GetStringAsync(TestController.ValuePath);
    }

    private static async Task<bool> Ping(int port)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{port}")
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
        public ValueTask<string> GetValue()
        {
            return ValueTask.FromResult(Value);
        }
    }

    private class TestMockServiceContract
    {
        public TestMockServiceContract(int webApiPort, TestController controller)
        {
            WebApiPort = webApiPort;
            Controller = controller;
        }

        public int WebApiPort { get; }

        public TestController Controller { get; }
    }

    private class TestMockService : MockWebApplicationService<TestMockServiceContract>
    {
        public TestMockService(IPortManager portManager, string value, Range<int> portRange)
            : base(portManager, portRange)
        {
            var controller = new TestController(value);
            Contract = new TestMockServiceContract(WebApiPort.Number, controller);
            AddController(controller);
        }

        public TestMockService(IPortManager portManager, string value)
            : this(portManager, value, PortRange.Default)
        {
        }

        public override TestMockServiceContract Contract { get; }
    }
}
