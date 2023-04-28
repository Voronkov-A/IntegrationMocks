using IntegrationMocks.Sample.Locations.Adapters.WebApi.Queries;
using IntegrationMocks.Sample.Locations.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationMocks.Sample.Locations.Adapters.WebApi;

public class LocationsController : LocationsControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ActionResult<CreateLocationResponse>> Create(
        CreateLocationRequest body,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(new CreateLocationCommand(body.Name), cancellationToken);
        var link = Url.ActionLink(nameof(Get), "Locations", new { id })
                   ?? throw new InvalidOperationException("Null action link.");
        return Created(
            link,
            new CreateLocationResponse()
            {
                Id = id
            });
    }

    public override async Task<ActionResult<LocationView>> Get(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var location = await _mediator.Send(new GetLocationQuery(id), cancellationToken);

        if (location != null)
        {
            return location;
        }

        return NotFound();
    }
}
