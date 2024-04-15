using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Application.Common;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
