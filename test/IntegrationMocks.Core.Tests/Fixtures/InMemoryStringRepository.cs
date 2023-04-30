using System.Collections.Concurrent;
using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.Tests.Fixtures;

public class InMemoryStringRepository : IStringRepository
{
    private readonly ConcurrentDictionary<string, string> _items;

    public InMemoryStringRepository()
    {
        _items = new ConcurrentDictionary<string, string>();
    }

    public HashSet<string> GetAll()
    {
        return _items.Keys.ToHashSet();
    }

    public bool Add(string value)
    {
        return _items.TryAdd(value, value);
    }

    public void Remove(string value)
    {
        _items.Remove(value, out _);
    }
}
