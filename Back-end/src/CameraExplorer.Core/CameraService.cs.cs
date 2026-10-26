namespace CameraExplorer.Core;

/// <summary>
/// Application logic shared by the CLI and the API.
/// </summary>
public sealed class CameraService(ICameraRepository repository)
{
    public IReadOnlyList<Camera> GetAll() =>
        repository.GetAll().OrderBy(c => c.Number).ToList();

    /// <summary>
    /// Case-insensitive "contains" search on the camera name.
    /// A null or blank term returns all cameras.
    /// </summary>
    public IReadOnlyList<Camera> Search(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GetAll();

        var term = name.Trim();
        return GetAll()
            .Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IReadOnlyList<CameraLoadIssue> GetLoadIssues() => repository.GetLoadIssues();
}