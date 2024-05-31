using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Modules.Testcontainers;

public static class ContainerExtensions
{
    public static async Task StartOrInspectAsync(
        this IContainer container,
        string containerName,
        CancellationToken cancellationToken)
    {
        try
        {
            await container.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var (stdOut, stdErr) = await GetContainerLogs(containerName, cancellationToken);
            throw new ContainerStartException("Could not start container.", stdOut, stdErr, ex);
        }
    }

    private static async Task<(string StdOut, string StdErr)> GetContainerLogs(
        string containerName,
        CancellationToken cancellationToken)
    {
        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig
            .GetDockerClientConfiguration()
            .CreateClient();
        var container = (await dockerClient.Containers
            .ListContainersAsync(
                new ContainersListParameters
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        ["name"] = new Dictionary<string, bool>
                        {
                            [containerName] = true
                        }
                    },
                    All = true
                },
                cancellationToken))
            .FirstOrDefault(x => x.Names.Any(x => x == $"/{containerName}" || x == containerName));

        if (container == null)
        {
            return ("", "");
        }

        using var logs = await dockerClient.Containers.GetContainerLogsAsync(
            container.ID,
            tty: false,
            new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true
            },
            cancellationToken);
        return await logs.ReadOutputToEndAsync(cancellationToken);
    }
}
