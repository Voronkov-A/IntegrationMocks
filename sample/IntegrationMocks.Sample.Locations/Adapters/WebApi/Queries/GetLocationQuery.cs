using MediatR;
using System;

namespace IntegrationMocks.Sample.Locations.Adapters.WebApi.Queries;

internal sealed record GetLocationQuery(Guid LocationId) : IRequest<LocationView?>;
