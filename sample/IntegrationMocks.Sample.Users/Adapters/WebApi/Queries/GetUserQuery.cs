using MediatR;
using System;

namespace IntegrationMocks.Sample.Users.Adapters.WebApi.Queries;

internal sealed record GetUserQuery(Guid LocationId) : IRequest<UserView?>;
