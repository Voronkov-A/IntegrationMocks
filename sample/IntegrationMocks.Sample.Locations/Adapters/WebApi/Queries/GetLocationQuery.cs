using MediatR;

namespace IntegrationMocks.Sample.Locations.Adapters.WebApi.Queries;

public record GetLocationQuery(Guid LocationId) : IRequest<LocationView?>;
