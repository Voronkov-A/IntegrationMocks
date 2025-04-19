using System;

namespace IntegrationMocks.Core.Environments;

public sealed class EnvironmentVariable
{
    public EnvironmentVariable(string name, string? defaultValue = null)
    {
        Name = name;
        DefaultValue = defaultValue;
    }

    public string Name { get; }

    public string? DefaultValue { get; }

    public string GetValue()
    {
        return GetValueOrDefault() ?? throw new InvalidCastException($"Environment variable {Name} is not defined.");
    }

    public string? GetValueOrDefault()
    {
        return Environment.GetEnvironmentVariable(Name) ?? DefaultValue;
    }
}
