using System;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

internal sealed class UsersHostServiceContract
{
    public required Uri WebApiUrl { get; init; }
}
