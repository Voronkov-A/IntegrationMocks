using IntegrationMocks.Sample.Locations.Domain;
using MediatR;

namespace IntegrationMocks.Sample.Locations.Adapters.WebApi.Queries;

public class GetLocationQueryHandler : IRequestHandler<GetLocationQuery, LocationView?>
{
    private readonly ILocationRepository _repository;

    public GetLocationQueryHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LocationView?> Handle(GetLocationQuery request, CancellationToken cancellationToken)
    {
        var location = await _repository.Find(request.LocationId, cancellationToken);
        return location == null
            ? null
            : new LocationView
            {
                Id = location.Id,
                Name = location.Name
            };
    }
}
