using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence;

public class PersistenceMigrator : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public PersistenceMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<PersistenceContext>().Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
