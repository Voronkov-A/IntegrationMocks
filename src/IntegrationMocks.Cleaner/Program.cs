using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.FluentDocker;
using IntegrationMocks.Core.Networking;

namespace IntegrationMocks.Cleaner;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var command = new RootCommand("Clean resources used by roughly interrupted IntegrationMocks.");
        command.SetHandler(async () => await Clean(CancellationToken.None));

        var parser = new CommandLineBuilder(command)
            .UseVersionOption()
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .CancelOnProcessTermination()
            .Build();

        await parser.InvokeAsync(args);
    }

    private static async Task Clean(CancellationToken cancellationToken)
    {
        await FluentDockerContainerManager.Default.DeleteAllContainers(cancellationToken);
        PortManager.Default.DeleteAllPorts();
    }
}
