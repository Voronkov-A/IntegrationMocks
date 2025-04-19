using IntegrationMocks.Sample.Users.Application.Common;
using MediatR;
using System;

namespace IntegrationMocks.Sample.Users.Application.Commands;

internal sealed record CreateUserCommand(string Name, Guid LocationId) :
    IRequest<CommandResult<Guid>>,
    ICommand;
