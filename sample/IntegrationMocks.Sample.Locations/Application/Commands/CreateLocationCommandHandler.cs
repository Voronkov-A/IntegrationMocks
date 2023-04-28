using IntegrationMocks.Sample.Locations.Domain;
using MediatR;

namespace IntegrationMocks.Sample.Locations.Application.Commands;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Guid>
{
    private readonly ILocationFactory _factory;
    private readonly ILocationRepository _repository;

    public CreateLocationCommandHandler(ILocationFactory factory, ILocationRepository repository)
    {
        _factory = factory;
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = _factory.CreateLocation(request.Name);
        await _repository.Add(location, cancellationToken);
        return location.Id;
    }
}
