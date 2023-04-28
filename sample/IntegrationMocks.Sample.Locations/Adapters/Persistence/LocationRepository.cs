using IntegrationMocks.Sample.Locations.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationMocks.Sample.Locations.Adapters.Persistence;

public class LocationRepository : ILocationRepository
{
    private readonly PersistenceContext _context;

    public LocationRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task Add(Location item, CancellationToken cancellationToken)
    {
        await _context.Locations.AddAsync(item, cancellationToken);
    }

    public async Task<Location?> Find(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Locations.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
