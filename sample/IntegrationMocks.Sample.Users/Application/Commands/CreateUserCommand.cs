using IntegrationMocks.Sample.Users.Application.Common;
using IntegrationMocks.Sample.Users.Domain.Common;
using MediatR;

namespace IntegrationMocks.Sample.Users.Application.Commands;

public record CreateUserCommand(string Name, Guid LocationId) : IRequest<CommandResult<Guid>>, ICommand;
