namespace IntegrationMocks.Sample.Users.Domain;

public interface ILocationRepository
{
    Task<Location?> Find(Guid id, CancellationToken cancellationToken);
}
