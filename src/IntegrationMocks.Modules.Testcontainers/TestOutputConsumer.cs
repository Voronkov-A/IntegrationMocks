using DotNet.Testcontainers.Configurations;
using IntegrationMocks.Modules.Testcontainers.Streams;
using System;
using System.IO;
using System.Text;

namespace IntegrationMocks.Modules.Testcontainers;

/// <summary>
/// Writes to Console and Debug.
/// </summary>
public sealed class TestOutputConsumer : IOutputConsumer
{
    public bool Enabled => true;

    public Stream Stdout { get; } = new CompositeOutputStream(new[]
    {
        Console.OpenStandardOutput(),
        new DebugOutputStream(new UTF8Encoding(false))
    });

    public Stream Stderr { get; } = new CompositeOutputStream(new[]
    {
        Console.OpenStandardOutput(),
        new DebugOutputStream(new UTF8Encoding(false))
    });

    public void Dispose()
    {
        Stdout?.Dispose();
        Stderr?.Dispose();
    }
}
