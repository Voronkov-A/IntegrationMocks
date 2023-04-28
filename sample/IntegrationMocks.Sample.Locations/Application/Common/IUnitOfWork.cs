namespace IntegrationMocks.Sample.Locations.Application.Common;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
