using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Locations.Domain;

public interface ILocationRepository
{
    Task Add(Location item, CancellationToken cancellationToken);

    Task<Location?> Find(Guid id, CancellationToken cancellationToken);
}
