using IntegrationMocks.Sample.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence;

public class UserRepository : IUserRepository
{
    private readonly PersistenceContext _context;

    public UserRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task Add(User item, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(item, cancellationToken);
    }

    public async Task<User?> Find(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
