namespace IntegrationMocks.Core.Miscellaneous;

public static class RandomName
{
    public static string PrefixPidGuid(string prefix)
    {
        return $"{prefix}_{Environment.ProcessId}_{System.Guid.NewGuid():N}";
    }

    public static string PrefixGuid(string prefix)
    {
        return $"{prefix}_{System.Guid.NewGuid():N}";
    }

    public static string Guid()
    {
        return System.Guid.NewGuid().ToString("N");
    }
}
