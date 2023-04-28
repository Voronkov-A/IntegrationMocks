namespace IntegrationMocks.Core.Resources;

public interface IStringRepository
{
    HashSet<string> GetAll();

    bool Add(string value);

    void Remove(string value);
}
