using IntegrationMocks.Core.Miscellaneous;
using IntegrationMocks.Core.Resources;

namespace IntegrationMocks.Core.Docker;

public abstract class DockerInfrastructureService<TContract> : IInfrastructureService<TContract>
{
    private readonly IDockerContainerManager _containerManager;
    private readonly INameGenerator _nameGenerator;
    private IDockerContainer? _container;
    private int _disposed;

    protected DockerInfrastructureService(IDockerContainerManager containerManager, INameGenerator nameGenerator)
    {
        _containerManager = containerManager;
        _nameGenerator = nameGenerator;
    }

    public abstract TContract Contract { get; }

    public virtual async Task InitializeAsync(CancellationToken cancellationToken)
    {
        DisposeFlag.Check(ref _disposed, this);

        if (_container != null)
        {
            return;
        }

        try
        {
            _container = await _containerManager.StartContainer(_nameGenerator, ConfigureContainer, cancellationToken);
            await WaitUntilReady(_container, cancellationToken);
        }
        catch when (_container != null)
        {
            _container.Dispose();
            _container = null;
            throw;
        }
    }

    public void Dispose()
    {
        using (NullSynchronizationContext.Enter())
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void ConfigureContainer(IDockerContainerBuilder builder);

    protected virtual Task WaitUntilReady(IDockerContainer container, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!DisposeFlag.Mark(ref _disposed))
        {
            return ValueTask.CompletedTask;
        }

        _container?.Dispose();
        return ValueTask.CompletedTask;
    }
}
