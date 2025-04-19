namespace IntegrationMocks.Core.Environments;

public interface IServiceBinding<out TContract>
{
    IInfrastructureService<TContract>? GetOrDefault();
}
