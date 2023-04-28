using IntegrationMocks.Sample.Users.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence.Common;

public class DbContextUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;

    public DbContextUnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
