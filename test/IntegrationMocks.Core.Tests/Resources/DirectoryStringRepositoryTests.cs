using AutoFixture;
using IntegrationMocks.Core.Resources;
using Xunit;

namespace IntegrationMocks.Core.Tests.Resources;

public sealed class DirectoryStringRepositoryTests : IDisposable
{
    private readonly IFixture _fixture;
    private readonly string _directoryPath;
    private readonly DirectoryStringRepository _sut;

    public DirectoryStringRepositoryTests()
    {
        _fixture = new Fixture();
        _directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"{nameof(IntegrationMocks)}_{nameof(DirectoryStringRepositoryTests)}");
        if (Directory.Exists(_directoryPath))
        {
            Directory.Delete(_directoryPath, recursive: true);
        }
        _sut = new DirectoryStringRepository(_directoryPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directoryPath))
        {
            Directory.Delete(_directoryPath, recursive: true);
        }
    }

    [Fact]
    public void Add_creates_file()
    {
        var value = _fixture.Create<string>();

        var added = _sut.Add(value);

        Assert.True(added);
        Assert.True(File.Exists(Path.Combine(_directoryPath, value)));
    }

    [Fact]
    public void Add_returns_false_on_duplicate()
    {
        var value = _fixture.Create<string>();
        _sut.Add(value);

        var added = _sut.Add(value);

        Assert.False(added);
    }

    [Fact]
    public void Remove_deletes_previously_created_file()
    {
        var value = _fixture.Create<string>();
        _sut.Add(value);

        _sut.Remove(value);

        Assert.False(File.Exists(Path.Combine(_directoryPath, value)));
    }

    [Fact]
    public void Remove_does_not_throw_on_nonexistent_file()
    {
        var value = _fixture.Create<string>();

        var exception = Record.Exception(() => _sut.Remove(value));

        Assert.Null(exception);
    }

    [Fact]
    public void GetAll_nothing_by_default()
    {
        var values = _sut.GetAll();

        Assert.Empty(values);
    }

    [Fact]
    public void GetAll_returns_added_values()
    {
        var expectedValues = _fixture.CreateMany<string>().ToHashSet();
        foreach (var value in expectedValues)
        {
            _sut.Add(value);
        }

        var actualValues = _sut.GetAll();

        Assert.Equal(expectedValues, actualValues);
    }

    [Fact]
    public void GetAll_does_not_return_removed_values()
    {
        var expectedValues = _fixture.CreateMany<string>().ToHashSet();
        var removedValues = _fixture.CreateMany<string>().ToList();
        var allValues = expectedValues.Concat(removedValues);
        foreach (var value in allValues)
        {
            _sut.Add(value);
        }
        foreach (var value in removedValues)
        {
            _sut.Remove(value);
        }

        var actualValues = _sut.GetAll();

        Assert.Equal(expectedValues, actualValues);
    }
}
