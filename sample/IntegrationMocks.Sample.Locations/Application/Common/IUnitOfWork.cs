using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Locations.Application.Common;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
