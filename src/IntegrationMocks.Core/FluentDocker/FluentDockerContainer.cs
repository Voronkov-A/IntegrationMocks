using System.Diagnostics;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services.Extensions;
using IntegrationMocks.Core.Docker;
using IntegrationMocks.Core.Execution;
using IntegrationMocks.Core.Miscellaneous;

namespace IntegrationMocks.Core.FluentDocker;

public sealed class FluentDockerContainer : IDockerContainer
{
    private readonly IFluentDockerContainerHandle _handle;
    private int _disposed;

    public FluentDockerContainer(IFluentDockerContainerHandle handle)
    {
        _handle = handle;
    }

    public DockerContainerState State
    {
        get
        {
            DisposeFlag.Check(ref _disposed, this);

            var state = _handle.ContainerService.GetConfiguration(fresh: true).State;

            if (!string.IsNullOrWhiteSpace(state.Error))
            {
                throw new SystemException($"Unable to start container: {state.Error}");
            }

            return state.ToServiceState().ToDockerContainerState();
        }
    }

    public List<ProcessDescriptor> GetAllProcesses()
    {
        return _handle.ContainerService
            .GetRunningProcesses()
            .Rows
            .Select(x => new ProcessDescriptor(x.Pid, x.Command))
            .ToList();
    }

    public Process StartProcess(ProcessStartOptions options)
    {
        DisposeFlag.Check(ref _disposed, this);

        var actualCommand = "docker".ResolveBinary();
        var actualArguments = new[]
        {
            "exec",
            "-i", _handle.ContainerService.Id
        }.Append(options.Command).Concat(options.Arguments);

        var startInfo = new ProcessStartInfo()
        {
            CreateNoWindow = options.CreateNoWindow,
            RedirectStandardOutput = options.RedirectStandardOutput,
            RedirectStandardError = options.RedirectStandardError,
            UseShellExecute = options.UseShellExecute,
            FileName = actualCommand
        };

        foreach (var arg in actualArguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        Process? process = null;

        try
        {
            process = new Process
            {
                StartInfo = startInfo
            };

            if (!process.Start())
            {
                throw new SystemException($"Could not start process '{actualCommand}'.");
            }

            return process;
        }
        catch
        {
            process?.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (DisposeFlag.Mark(ref _disposed))
        {
            _handle.Dispose();
        }
    }
}
