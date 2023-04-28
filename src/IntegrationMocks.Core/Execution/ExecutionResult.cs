namespace IntegrationMocks.Core.Execution;

public class ExecutionResult
{
    public ExecutionResult(int exitCode)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}
