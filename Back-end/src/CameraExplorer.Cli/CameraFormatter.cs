using System.Globalization;
using CameraExplorer.Core;

namespace CameraExplorer.Cli;

public static class CameraFormatter
{
    /// <summary>Formats a camera as "501 | UTR-CM-501 Neude ... | 52.093421 | 5.118278".</summary>
    public static string Format(Camera camera) =>
        string.Create(CultureInfo.InvariantCulture,
            $"{camera.Number} | {camera.Name} | {camera.Latitude} | {camera.Longitude}");
}