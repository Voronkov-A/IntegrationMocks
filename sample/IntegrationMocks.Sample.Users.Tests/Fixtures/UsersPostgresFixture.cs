using IntegrationMocks.Core;
using IntegrationMocks.Core.Environments;
using IntegrationMocks.Core.FluentDocker;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Library.Sql;
using IntegrationMocks.Sample.Users.Adapters.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public sealed class UsersPostgresFixture : IAsyncLifetime, IDisposable
{
    private readonly string _persistenceConnectionString;

    public UsersPostgresFixture()
    {
        Postgres = new BindingInfrastructureService<SqlServiceContract>(
            "GITLAB_CI",
            ServiceBinding.Create("true", () => new EnvironmentSqlService()),
            ServiceBinding.Create(() => new DockerPostgresService(new FluentDockerContainerManager(
                LoggerFixture.CreateLogger<FluentDockerContainerManager>()))));
        _persistenceConnectionString = Postgres.CreatePostgresConnectionString(
            RandomName.PrefixPidGuid(nameof(UsersPostgresFixture)));
    }

    public IInfrastructureService<SqlServiceContract> Postgres { get; }

    public async Task InitializeAsync()
    {
        await Postgres.InitializeAsync();

        await using var context = CreatePersistenceContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = CreatePersistenceContext();
        await context.Database.EnsureDeletedAsync();

        await Postgres.DisposeAsync();
    }

    public void Dispose()
    {
        Postgres.Dispose();
    }

    public PersistenceContext CreatePersistenceContext()
    {
        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseNpgsql(_persistenceConnectionString)
            .Options;
        return new PersistenceContext(options);
    }
}
