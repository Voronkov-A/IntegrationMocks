using System.Collections.Generic;

namespace IntegrationMocks.Core.Networking;

public interface IPortNumberRepository
{
    HashSet<int> GetAll();

    bool Add(int value);

    void Remove(int value);
}
