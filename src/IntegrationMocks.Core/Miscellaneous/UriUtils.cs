namespace IntegrationMocks.Core.Miscellaneous;

public static class UriUtils
{
    public static Uri HttpLocalhost(int port)
    {
        return new Uri($"http://localhost:{port}");
    }
}
