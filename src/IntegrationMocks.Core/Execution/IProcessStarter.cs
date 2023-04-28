using System.Diagnostics;

namespace IntegrationMocks.Core.Execution;

public interface IProcessStarter
{
    Process StartProcess(ProcessStartOptions options);
}
