using IntegrationMocks.Sample.Users.Application.Common;
using IntegrationMocks.Sample.Users.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationMocks.Sample.Users.Application.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CommandResult<Guid>>
{
    private readonly IUserFactory _userFactory;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateUserCommandHandler(
        IUserFactory userFactory,
        IUserRepository userRepository,
        ILocationRepository locationRepository)
    {
        _userFactory = userFactory;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CommandResult<Guid>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var location = await _locationRepository.Find(request.LocationId, cancellationToken);

        if (location == null)
        {
            return CommandResult.Fail<Guid>("Invalid location identity.");
        }

        var user = _userFactory.CreateUser(request.Name, location.Value);
        await _userRepository.Add(user, cancellationToken);
        return CommandResult.Success(user.Id);
    }
}
