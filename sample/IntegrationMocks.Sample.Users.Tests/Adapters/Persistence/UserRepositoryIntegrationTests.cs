using AutoFixture;
using IntegrationMocks.Sample.Users.Adapters.Persistence;
using IntegrationMocks.Sample.Users.Domain;
using IntegrationMocks.Sample.Users.Tests.Fixtures;
using IntegrationMocks.Sample.Users.Tests.Fixtures.Customizations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationMocks.Sample.Users.Tests.Adapters.Persistence;

public class UserRepositoryIntegrationTests : IClassFixture<UsersPostgresFixture>
{
    private readonly UsersPostgresFixture _postgres;
    private readonly IFixture _fixture;

    public UserRepositoryIntegrationTests(UsersPostgresFixture postgres)
    {
        _postgres = postgres;
        _fixture = new Fixture().Customize(new LocationCustomization()).Customize(new UserCustomization());
    }

    [Fact]
    public async Task Add_inserts_data_into_table()
    {
        var expectedItem = _fixture.Create<User>();
        await using var writeContext = _postgres.CreatePersistenceContext();
        var sut = CreateRepository(writeContext);

        await sut.Add(expectedItem, CancellationToken.None);
        await writeContext.SaveChangesAsync();

        await using var readContext = _postgres.CreatePersistenceContext();
        var actualItem = await readContext.Users.SingleAsync(x => x.Id == expectedItem.Id);
        Assert.Equal(expectedItem.Id, actualItem.Id);
        Assert.Equal(expectedItem.Name, actualItem.Name);
        Assert.Equal(expectedItem.Location, actualItem.Location);
    }

    [Fact]
    public async Task Add_throws_on_duplicate()
    {
        var item = _fixture.Create<User>();
        await using var writeContext = _postgres.CreatePersistenceContext();
        var sut = CreateRepository(writeContext);
        await sut.Add(item, CancellationToken.None);
        await writeContext.SaveChangesAsync();

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await sut.Add(item, CancellationToken.None);
            await writeContext.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task Find_returns_previously_added_item()
    {
        var expectedItem = _fixture.Create<User>();
        await using var writeContext = _postgres.CreatePersistenceContext();
        var writeRepo = CreateRepository(writeContext);
        await writeRepo.Add(expectedItem, CancellationToken.None);
        await writeContext.SaveChangesAsync();
        await using var readContext = _postgres.CreatePersistenceContext();
        var sut = CreateRepository(readContext);

        var actualItem = await sut.Find(expectedItem.Id, CancellationToken.None);

        Assert.NotNull(actualItem);
        Assert.Equal(expectedItem.Id, actualItem.Id);
        Assert.Equal(expectedItem.Name, actualItem.Name);
        Assert.Equal(expectedItem.Location, actualItem.Location);
    }

    [Fact]
    public async Task Find_returns_nothing_for_nonexistent_item()
    {
        await using var readContext = _postgres.CreatePersistenceContext();
        var sut = CreateRepository(readContext);
        var id = _fixture.Create<Guid>();

        var actualItem = await sut.Find(id, CancellationToken.None);

        Assert.Null(actualItem);
    }

    private static UserRepository CreateRepository(PersistenceContext context)
    {
        return new UserRepository(context);
    }
}
