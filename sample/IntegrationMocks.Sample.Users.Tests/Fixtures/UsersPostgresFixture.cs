using IntegrationMocks.Core;
using IntegrationMocks.Core.Environments;
using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Modules.Postgres;
using IntegrationMocks.Sample.Users.Adapters.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public sealed class UsersPostgresFixture : IAsyncLifetime, IDisposable
{
    private readonly string _persistenceConnectionString;

    public UsersPostgresFixture()
    {
        Postgres = new BindingInfrastructureService<PostgresServiceContract>(
            ServiceBinding.Create("GITLAB_CI", "true", () => new EnvironmentPostgresService()),
            ServiceBinding.Create(() => new DockerPostgresService()));
        _persistenceConnectionString = Postgres.CreatePostgresConnectionString(
            RandomName.PrefixPidGuid(nameof(UsersPostgresFixture)));
    }

    internal IInfrastructureService<PostgresServiceContract> Postgres { get; }

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

    internal PersistenceContext CreatePersistenceContext()
    {
        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseNpgsql(_persistenceConnectionString)
            .Options;
        return new PersistenceContext(options);
    }
}
