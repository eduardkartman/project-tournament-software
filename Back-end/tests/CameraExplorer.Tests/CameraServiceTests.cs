using CameraExplorer.Core;

namespace CameraExplorer.Tests;

public class CameraServiceTests
{
    private sealed class FakeRepository(params Camera[] cameras) : ICameraRepository
    {
        public IReadOnlyList<Camera> GetAll() => cameras;

        public IReadOnlyList<CameraLoadIssue> GetLoadIssues() => [];
    }

    private static CameraService CreateService() => new(new FakeRepository(
        new Camera(702, "UTR-CM-702 Oudegracht", 52.09, 5.12),
        new Camera(501, "UTR-CM-501 Neude rijbaan", 52.093, 5.118),
        new Camera(503, "UTR-CM-503 Neude plein", 52.093, 5.118)));

    [Fact]
    public void Search_IsCaseInsensitiveAndMatchesPartOfName()
    {
        var result = CreateService().Search("nEUDe");

        Assert.Equal([501, 503], result.Select(c => c.Number));
    }

    [Fact]
    public void Search_NoMatch_ReturnsEmpty()
    {
        Assert.Empty(CreateService().Search("does-not-exist"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Search_BlankTerm_ReturnsAllCameras(string? term)
    {
        Assert.Equal(3, CreateService().Search(term).Count);
    }

    [Fact]
    public void GetAll_IsOrderedByCameraNumber()
    {
        var result = CreateService().GetAll();

        Assert.Equal([501, 503, 702], result.Select(c => c.Number));
    }
}