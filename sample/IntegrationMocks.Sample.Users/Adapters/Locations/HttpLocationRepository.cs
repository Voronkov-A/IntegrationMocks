using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using IntegrationMocks.Sample.Users.Domain;

namespace IntegrationMocks.Sample.Users.Adapters.Locations;

public sealed class HttpLocationRepository : ILocationRepository, IDisposable
{
    private readonly HttpClient _client;

    public HttpLocationRepository(LocationsOptions options)
    {
        _client = new HttpClient()
        {
            BaseAddress = options.BaseAddress
        };
    }

    public async Task<Location?> Find(Guid id, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync($"/api/locations/{id}", cancellationToken);

        return response.StatusCode switch
        {
            HttpStatusCode.OK =>
                new Location(
                    (await response.Content.ReadFromJsonAsync<LocationView>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Null content.")).Id),
            HttpStatusCode.NotFound => null,
            _ => throw new InvalidOperationException($"Unexpected status code: {response.StatusCode}.")
        };
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class LocationView
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }
    }
}
