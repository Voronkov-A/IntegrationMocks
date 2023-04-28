using IntegrationMocks.Sample.Locations.Application.Common;
using MediatR;

namespace IntegrationMocks.Sample.Locations.Application.Commands;

public record CreateLocationCommand(string Name) : IRequest<Guid>, ICommand;
