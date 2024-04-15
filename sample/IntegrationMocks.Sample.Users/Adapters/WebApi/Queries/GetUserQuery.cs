using MediatR;
using System;

namespace IntegrationMocks.Sample.Users.Adapters.WebApi.Queries;

public record GetUserQuery(Guid LocationId) : IRequest<UserView?>;
