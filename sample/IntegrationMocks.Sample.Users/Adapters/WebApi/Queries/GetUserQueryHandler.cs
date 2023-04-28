using IntegrationMocks.Sample.Users.Domain;
using MediatR;

namespace IntegrationMocks.Sample.Users.Adapters.WebApi.Queries;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserView?>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserView?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.Find(request.LocationId, cancellationToken);
        return user == null
            ? null
            : new UserView
            {
                Id = user.Id,
                Name = user.Name,
                LocationId = user.Location.Id
            };
    }
}
