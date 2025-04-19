using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationMocks.Core.Environments;

public class BindingInfrastructureService<TContract> : DecoratingInfrastructureService<TContract>
{
    public BindingInfrastructureService(params IServiceBinding<TContract>[] bindings) : base(CreateInner(bindings))
    {
    }

    private static IInfrastructureService<TContract> CreateInner(
        IReadOnlyCollection<IServiceBinding<TContract>> bindings)
    {
        var service = bindings.Select(x => x.GetOrDefault()).FirstOrDefault(x => x != null);
        return service
               ?? throw new InvalidOperationException($"All the bindings have failed for service {typeof(TContract)}.");
    }
}
