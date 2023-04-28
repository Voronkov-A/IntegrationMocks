namespace IntegrationMocks.Sample.Users.Domain;

public interface IUserRepository
{
    Task Add(User item, CancellationToken cancellationToken);

    Task<User?> Find(Guid id, CancellationToken cancellationToken);
}
