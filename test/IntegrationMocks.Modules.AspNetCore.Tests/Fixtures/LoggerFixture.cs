using Microsoft.Extensions.Logging;

namespace IntegrationMocks.Modules.AspNetCore.Tests.Fixtures;

internal static class LoggerFixture
{
    private static readonly ILoggerFactory LoggerFactory
        = Microsoft.Extensions.Logging.LoggerFactory.Create(x => x
            .SetMinimumLevel(LogLevel.Debug)
            .AddConsole());

    public static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }
}
