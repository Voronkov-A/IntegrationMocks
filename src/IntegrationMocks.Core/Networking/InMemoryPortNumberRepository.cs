using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationMocks.Core.Networking;

/// <summary>
/// Warning: this implementation assumes that we have exactly one test process in time.
/// </summary>
public class InMemoryPortNumberRepository : IPortNumberRepository
{
    private readonly ConcurrentDictionary<int, int> _items;

    public InMemoryPortNumberRepository()
    {
        _items = new ConcurrentDictionary<int, int>();
    }

    public bool Add(int value)
    {
        return _items.TryAdd(value, value);
    }

    public HashSet<int> GetAll()
    {
        return _items.Keys.ToHashSet();
    }

    public void Remove(int value)
    {
        _items.TryRemove(value, out _);
    }
}
