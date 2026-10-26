namespace CameraExplorer.Core;

/// <summary>A source line that could not be turned into a camera.</summary>
public sealed record CameraLoadIssue(int LineNumber, string Line, string Reason);

/// <summary>The cameras that were loaded, plus the rows that were skipped and why.</summary>
public sealed record CameraLoadResult(
    IReadOnlyList<Camera> Cameras,
    IReadOnlyList<CameraLoadIssue> Issues);