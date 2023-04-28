namespace IntegrationMocks.Core.Environments;

public class BindingInfrastructureService<TContract> : DecoratingInfrastructureService<TContract>
{
    public BindingInfrastructureService(
        string environmentVariableName,
        params ServiceBinding<TContract>[] bindings)
        : base(CreateInner(environmentVariableName, bindings))
    {
    }

    public BindingInfrastructureService(params ServiceBinding<TContract>[] bindings)
        : base(CreateInner("ASPNETCORE_ENVIRONMENT", bindings))
    {
    }

    private static IInfrastructureService<TContract> CreateInner(
        string environmentVariableName,
        IReadOnlyCollection<ServiceBinding<TContract>> bindings)
    {
        var environmentName = Environment.GetEnvironmentVariable(environmentVariableName);
        var binding = bindings.LastOrDefault(x => x.EnvironmentName == environmentName)
                      ?? bindings.LastOrDefault(x => x.EnvironmentName == null);
        return binding == null
            ? throw new InvalidOperationException($"Could not find binding for environment '{environmentName ?? ""}'.")
            : binding.Factory();
    }
}
