using System.Net;
using System.Net.Http.Json;
using CameraExplorer.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CameraExplorer.Tests;

public class CamerasApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private sealed class FakeRepository : ICameraRepository
    {
        public IReadOnlyList<Camera> GetAll() =>
        [
            new Camera(702, "UTR-CM-702 Oudegracht", 52.09, 5.12),
            new Camera(501, "UTR-CM-501 Neude rijbaan", 52.093, 5.118),
            new Camera(503, "UTR-CM-503 Neude plein", 52.093, 5.118),
        ];

        public IReadOnlyList<CameraLoadIssue> GetLoadIssues() => [];
    }

    private HttpClient CreateClient() =>
        factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.AddSingleton<ICameraRepository>(new FakeRepository())))
            .CreateClient();

    [Fact]
    public async Task GetCameras_ReturnsAllCamerasOrderedByNumber()
    {
        var response = await CreateClient().GetAsync("/api/cameras");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cameras = await response.Content.ReadFromJsonAsync<List<Camera>>();
        Assert.Equal([501, 503, 702], cameras!.Select(c => c.Number));
    }

    [Fact]
    public async Task GetCameras_WithName_ReturnsOnlyMatches()
    {
        var cameras = await CreateClient().GetFromJsonAsync<List<Camera>>("/api/cameras?name=neude");

        Assert.Equal([501, 503], cameras!.Select(c => c.Number));
    }

    [Fact]
    public async Task GetCameras_WithUnmatchedName_ReturnsEmptyList()
    {
        var cameras = await CreateClient().GetFromJsonAsync<List<Camera>>("/api/cameras?name=zzzz");

        Assert.Empty(cameras!);
    }

    [Fact]
    public async Task GetCameras_AllowsTheFrontendOrigin()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/cameras");
        request.Headers.Add("Origin", "http://localhost:5173");

        var response = await CreateClient().SendAsync(request);

        Assert.Equal("http://localhost:5173", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
}