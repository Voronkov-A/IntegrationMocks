namespace IntegrationMocks.Sample.Users;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(typeof(Program).Assembly)
                .AddEnvironmentVariables())
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
            .Build()
            .RunAsync();
    }
}
