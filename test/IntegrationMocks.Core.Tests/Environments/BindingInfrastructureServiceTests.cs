using AutoFixture;
using IntegrationMocks.Core.Environments;
using IntegrationMocks.Core.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Core.Tests.Environments;

public sealed class BindingInfrastructureServiceTests
{
    private readonly IFixture _fixture;

    public BindingInfrastructureServiceTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_uses_environment_binding_when_defined()
    {
        var environmentVariableName = _fixture.Create<string>();
        var environmentToService = _fixture
            .CreateMany<KeyValuePair<string, TestInfrastructureService>>()
            .ToDictionary(x => x.Key, x => x.Value);
        var defaultService = new TestInfrastructureService();
        var bindings = environmentToService
            .Select(x => ServiceBinding.Create(environmentVariableName, x.Key, () => x.Value))
            .Append(ServiceBinding.Create(() => defaultService))
            .ToArray();
        var expectedEnvironment = environmentToService.Keys.Shuffle().First();
        var expectedService = environmentToService[expectedEnvironment];
        Environment.SetEnvironmentVariable(environmentVariableName, expectedEnvironment);

        using var sut = new BindingInfrastructureService<object>(bindings);

        Assert.Same(sut.Contract, expectedService.Contract);
    }

    [Fact]
    public void Constructor_uses_default_binding_when_others_do_not_match()
    {
        var environmentVariableName = _fixture.Create<string>();
        var environmentToService = _fixture
            .CreateMany<KeyValuePair<string, TestInfrastructureService>>()
            .ToDictionary(x => x.Key, x => x.Value);
        var defaultService = new TestInfrastructureService();
        var bindings = environmentToService
            .Select(x => ServiceBinding.Create(environmentVariableName, x.Key, () => x.Value))
            .Append(ServiceBinding.Create(() => defaultService))
            .ToArray();
        Environment.SetEnvironmentVariable(environmentVariableName, _fixture.Create<string>());

        using var sut = new BindingInfrastructureService<object>(bindings);

        Assert.Same(sut.Contract, defaultService.Contract);
    }

    [Fact]
    public void Constructor_uses_default_binding_when_environment_is_undefined()
    {
        var environmentToService = _fixture
            .CreateMany<KeyValuePair<string, TestInfrastructureService>>()
            .ToDictionary(x => x.Key, x => x.Value);
        var defaultService = new TestInfrastructureService();
        var bindings = environmentToService
            .Select(x => ServiceBinding.Create(_fixture.Create<string>(), x.Key, () => x.Value))
            .Append(ServiceBinding.Create(() => defaultService))
            .ToArray();

        using var sut = new BindingInfrastructureService<object>(bindings);

        Assert.Same(sut.Contract, defaultService.Contract);
    }

    [Fact]
    public void Constructor_throws_when_no_matching_binding_nor_default_found()
    {
        var environmentVariableName = _fixture.Create<string>();
        var environmentToService = _fixture
            .CreateMany<KeyValuePair<string, TestInfrastructureService>>()
            .ToDictionary(x => x.Key, x => x.Value);
        var bindings = environmentToService
            .Select(x => ServiceBinding.Create(_fixture.Create<string>(), x.Key, () => x.Value))
            .ToArray();
        Environment.SetEnvironmentVariable(environmentVariableName, _fixture.Create<string>());

        Assert.Throws<InvalidOperationException>(() => new BindingInfrastructureService<object>(bindings));
    }

    [Fact]
    public void Constructor_throws_when_binding_list_is_empty()
    {
        Assert.Throws<InvalidOperationException>(() => new BindingInfrastructureService<object>());
    }

    [Fact]
    public void Constructor_uses_first_matching_binding()
    {
        var environmentVariableName = _fixture.Create<string>();
        var expectedEnvironment = _fixture.Create<string>();
        var services = _fixture.CreateMany<TestInfrastructureService>().ToList();
        var bindings = services
            .Select(x => ServiceBinding.Create(environmentVariableName, expectedEnvironment, () => x))
            .ToArray();
        var expectedService = services.First();
        Environment.SetEnvironmentVariable(environmentVariableName, expectedEnvironment);

        using var sut = new BindingInfrastructureService<object>(bindings);

        Assert.Same(sut.Contract, expectedService.Contract);
    }

    [Fact]
    public void Constructor_uses_first_default_binding()
    {
        var services = _fixture.CreateMany<TestInfrastructureService>().ToList();
        var bindings = services.Select(x => ServiceBinding.Create(() => x)).ToArray();
        var expectedService = services.First();

        using var sut = new BindingInfrastructureService<object>(bindings);

        Assert.Same(sut.Contract, expectedService.Contract);
    }

    private sealed class TestInfrastructureService : IInfrastructureService<object>
    {
        public object Contract { get; } = new();

        public void Dispose()
        {
            // pass
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
