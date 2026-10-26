namespace CameraExplorer.Core;

public interface ICameraRepository
{
    IReadOnlyList<Camera> GetAll();

    /// <summary>Rows in the source that were skipped because they were invalid.</summary>
    IReadOnlyList<CameraLoadIssue> GetLoadIssues();
}