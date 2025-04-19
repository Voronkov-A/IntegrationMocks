using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence;

internal sealed class PersistenceMigrator : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public PersistenceMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<PersistenceContext>()
            .Database
            .MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
