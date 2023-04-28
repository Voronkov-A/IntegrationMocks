namespace IntegrationMocks.Core.Execution;

public static class ProcessStarterExtensions
{
    public static async Task<ExecutionResult> Execute(
        this IProcessStarter starter,
        string command,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken = default)
    {
        using var process = starter.StartProcess(new ProcessStartOptions(command, arguments)
        {
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        await process.WaitForExitAsync(cancellationToken);
        return new ExecutionResult(process.ExitCode);
    }
}
