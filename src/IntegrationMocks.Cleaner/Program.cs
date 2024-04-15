using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using IntegrationMocks.Core.Networking;

namespace IntegrationMocks.Cleaner;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var command = new RootCommand(
            "Clean resources used by roughly interrupted IntegrationMocks.");
        var portNumberRepositoryDirectoryOption = new Option<string>(
            new[] { "--port-number-repository-directory", "-p" },
            description: "Port number repository directory if not default.",
            getDefaultValue: () => PortManager.DefaultPortNumberRepositoryDirectoryPath)
        {
            IsRequired = false
        };
        command.AddOption(portNumberRepositoryDirectoryOption);

        command.SetHandler(ic => Clean(GetRequired(ic, portNumberRepositoryDirectoryOption)));

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

    private static void Clean(string portNumberRepositoryDirectory)
    {
        var repository = new DirectoryPortNumberRepository(portNumberRepositoryDirectory);
        var ports = repository.GetAll();

        foreach (var port in ports)
        {
            repository.Remove(port);
        }
    }

    private static string GetRequired(InvocationContext context, Option<string> option)
    {
        return context.ParseResult.GetValueForOption(option)
            ?? throw new ApplicationException($"Option {option.Name} is required.");
    }
}
