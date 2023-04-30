namespace IntegrationMocks.Core.Execution;

public class ProcessDescriptor
{
    public ProcessDescriptor(long pid, string command)
    {
        Pid = pid;
        Command = command;
    }

    public long Pid { get; }

    public string Command { get; }
}
