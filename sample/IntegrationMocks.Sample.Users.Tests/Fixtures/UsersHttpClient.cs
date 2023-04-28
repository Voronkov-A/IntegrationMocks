using System.Net;
using System.Net.Http.Json;
using IntegrationMocks.Sample.Users.Adapters.WebApi;

namespace IntegrationMocks.Sample.Users.Tests.Fixtures;

public sealed class UsersHttpClient : IDisposable
{
    private readonly HttpClient _client;

    public UsersHttpClient(Uri baseAddress)
    {
        _client = new HttpClient()
        {
            BaseAddress = baseAddress
        };
    }

    public async Task<CreateUserResponse> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        using var postResponse = await _client.PostAsJsonAsync("/api/users", request, cancellationToken);

        if (postResponse.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException($"Unexpected response status code: {postResponse.StatusCode}.");
        }

        return await postResponse
                   .Content
                   .ReadFromJsonAsync<CreateUserResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Null content.");
    }

    public async Task<UserView> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<UserView>($"/api/users/{id}", cancellationToken)
               ?? throw new InvalidOperationException("Null content.");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
