using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.Resources;

public class RandomNameGenerator : INameGenerator
{
    private readonly string _prefix;

    public RandomNameGenerator(string prefix)
    {
        _prefix = prefix;
    }

    public RandomNameGenerator()
    {
        _prefix = "";
    }

    public string GenerateName()
    {
        return _prefix.Length == 0 ? RandomName.Guid() : RandomName.PrefixGuid(_prefix);
    }
}
