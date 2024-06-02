#define DEBUG

namespace IntegrationMocks.Modules.Testcontainers.Streams;

internal partial class DebugOutputStream
{
    private static void DebugWrite(string message)
    {
        System.Diagnostics.Debug.Write(message);
    }
}
