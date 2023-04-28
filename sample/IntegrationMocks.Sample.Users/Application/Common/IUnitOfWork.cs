namespace IntegrationMocks.Sample.Users.Application.Common;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
