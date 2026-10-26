using System.Globalization;
using System.Text.RegularExpressions;
using CameraExplorer.Core;

namespace CameraExplorer.Infrastructure;

public static partial class CameraCsvParser
{
    private const char Delimiter = ';';

    // Matches the number after a prefix like "UTR-CM-":
    //   "UTR-CM-501 Neude plein"          -> 501
    //   "UTR-CM-544-Janskerkhof (plein)"  -> 544
    [GeneratedRegex(@"^(?:[A-Za-z]+-)+(\d+)")]
    private static partial Regex NumberPattern();

    /// <summary>
    /// Parses the lines of the CSV (the first line is the header). Invalid lines are not fatal:
    /// they are skipped and reported in the result.
    /// </summary>
    public static CameraLoadResult Parse(IEnumerable<string> lines)
    {
        var cameras = new List<Camera>();
        var issues = new List<CameraLoadIssue>();
        var lineNumber = 0;

        foreach (var line in lines)
        {
            lineNumber++;
            if (lineNumber == 1 || string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                cameras.Add(ParseLine(line));
            }
            catch (FormatException ex)
            {
                issues.Add(new CameraLoadIssue(lineNumber, line, ex.Message));
            }
        }

        return new CameraLoadResult(cameras, issues);
    }

    private static Camera ParseLine(string line)
    {
        var fields = line.Split(Delimiter);
        if (fields.Length != 3)
            throw new FormatException($"expected 3 fields but found {fields.Length}");

        var name = fields[0].Trim();
        var match = NumberPattern().Match(name);
        if (!match.Success)
            throw new FormatException($"cannot find a camera number in '{name}'");

        var number = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        var latitude = ParseCoordinate(fields[1], "latitude");
        var longitude = ParseCoordinate(fields[2], "longitude");

        return new Camera(number, name, latitude, longitude);
    }

    private static double ParseCoordinate(string value, string field)
    {
        if (!double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            throw new FormatException($"invalid {field} '{value}'");

        return result;
    }
}