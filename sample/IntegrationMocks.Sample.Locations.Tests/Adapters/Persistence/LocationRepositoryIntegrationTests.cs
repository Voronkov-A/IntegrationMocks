using AutoFixture;
using IntegrationMocks.Sample.Locations.Adapters.Persistence;
using IntegrationMocks.Sample.Locations.Domain;
using IntegrationMocks.Sample.Locations.Tests.Fixtures;
using IntegrationMocks.Sample.Locations.Tests.Fixtures.Customizations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationMocks.Sample.Locations.Tests.Adapters.Persistence;

public class LocationRepositoryIntegrationTests : IClassFixture<LocationsPostgresFixture>
{
    private readonly LocationsPostgresFixture _postgres;
    private readonly IFixture _fixture;

    public LocationRepositoryIntegrationTests(LocationsPostgresFixture postgres)
    {
        _postgres = postgres;
        _fixture = new Fixture().Customize(new LocationCustomization());
    }

    [Fact]
    public async Task Add_inserts_data_into_table()
    {
        var expectedItem = _fixture.Create<Location>();
        await using var writeContext = _postgres.CreatePersistenceContext();
        var sut = CreateRepository(writeContext);

        await sut.Add(expectedItem, CancellationToken.None);
        await writeContext.SaveChangesAsync();

        await using var readContext = _postgres.CreatePersistenceContext();
        var actualItem = await readContext.Locations.SingleAsync(x => x.Id == expectedItem.Id);
        Assert.Equal(expectedItem.Id, actualItem.Id);
        Assert.Equal(expectedItem.Name, actualItem.Name);
    }

    [Fact]
    public async Task Add_throws_on_duplicate()
    {
        var item = _fixture.Create<Location>();
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
        var expectedItem = _fixture.Create<Location>();
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

    private static LocationRepository CreateRepository(PersistenceContext context)
    {
        return new LocationRepository(context);
    }
}
