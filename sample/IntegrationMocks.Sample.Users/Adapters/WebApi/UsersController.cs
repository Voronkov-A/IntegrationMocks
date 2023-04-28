using IntegrationMocks.Sample.Users.Adapters.WebApi.Queries;
using IntegrationMocks.Sample.Users.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationMocks.Sample.Users.Adapters.WebApi;

public class UsersController : UsersControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ActionResult<CreateUserResponse>> Create(
        CreateUserRequest body,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CreateUserCommand(body.Name, body.LocationId), cancellationToken);

        if (!result.IsSucceeded)
        {
            return BadRequest();
        }

        var id = result.GetOrThrow();
        var link = Url.ActionLink(nameof(Get), "Users", new { id })
                   ?? throw new InvalidOperationException("Null action link.");
        return Created(
            link,
            new CreateUserResponse()
            {
                Id = id
            });
    }

    public override async Task<ActionResult<UserView>> Get(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _mediator.Send(new GetUserQuery(id), cancellationToken);

        if (user != null)
        {
            return user;
        }

        return NotFound();
    }
}
