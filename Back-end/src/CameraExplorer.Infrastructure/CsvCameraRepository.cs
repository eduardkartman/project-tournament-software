using CameraExplorer.Core;

namespace CameraExplorer.Infrastructure;

/// <summary>Reads cameras from the CSV file once, on first use, and keeps them in memory.</summary>
public sealed class CsvCameraRepository(string csvPath) : ICameraRepository
{
    private readonly Lazy<CameraLoadResult> _result =
        new(() => CameraCsvParser.Parse(File.ReadLines(csvPath)));

    public IReadOnlyList<Camera> GetAll() => _result.Value.Cameras;

    public IReadOnlyList<CameraLoadIssue> GetLoadIssues() => _result.Value.Issues;
}