using System.Net;
using System.Net.Http.Json;
using IntegrationMocks.Sample.Locations.Adapters.WebApi;

namespace IntegrationMocks.Sample.Locations.Tests.Fixtures;

public sealed class LocationsHttpClient : IDisposable
{
    private readonly HttpClient _client;

    public LocationsHttpClient(Uri baseAddress)
    {
        _client = new HttpClient
        {
            BaseAddress = baseAddress
        };
    }

    public async Task<CreateLocationResponse> Create(
        CreateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var postResponse = await _client.PostAsJsonAsync("/api/locations", request, cancellationToken);

        if (postResponse.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException($"Unexpected response status code: {postResponse.StatusCode}.");
        }

        return await postResponse
                   .Content
                   .ReadFromJsonAsync<CreateLocationResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Null content.");
    }

    public async Task<LocationView> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<LocationView>($"/api/locations/{id}", cancellationToken)
            ?? throw new InvalidOperationException("Null content.");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
