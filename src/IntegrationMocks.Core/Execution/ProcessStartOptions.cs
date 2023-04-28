namespace IntegrationMocks.Core.Execution;

public class ProcessStartOptions
{
    public ProcessStartOptions(string command, IEnumerable<string> arguments)
    {
        Command = command;
        Arguments = arguments.ToList();
    }

    public string Command { get; }

    public IReadOnlyList<string> Arguments { get; }

    public bool CreateNoWindow { get; init; }

    public bool RedirectStandardOutput { get; init; }

    public bool RedirectStandardError { get; init; }

    public bool UseShellExecute { get; init; }
}
