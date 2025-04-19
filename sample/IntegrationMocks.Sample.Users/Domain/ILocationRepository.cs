using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Domain;

internal interface ILocationRepository
{
    Task<Location?> Find(Guid id, CancellationToken cancellationToken);
}
