using System.Globalization;
using CameraExplorer.Infrastructure;

namespace CameraExplorer.Tests;

public class CameraCsvParserTests
{
    private const string Header = "Camera;Latitude;Longitude";

    [Fact]
    public void Parse_ValidLine_MapsAllFields()
    {
        var result = CameraCsvParser.Parse([Header, "UTR-CM-503 Neude plein;52.093448;5.118536"]);

        var camera = Assert.Single(result.Cameras);
        Assert.Equal(503, camera.Number);
        Assert.Equal("UTR-CM-503 Neude plein", camera.Name);
        Assert.Equal(52.093448, camera.Latitude);
        Assert.Equal(5.118536, camera.Longitude);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void Parse_SkipsHeaderAndBlankLines()
    {
        var result = CameraCsvParser.Parse([Header, "", "UTR-CM-501 A;52.1;5.1", "  "]);

        Assert.Single(result.Cameras);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void Parse_UsesInvariantCulture_EvenWhenCurrentCultureUsesCommaDecimals()
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ro-RO");
        try
        {
            var result = CameraCsvParser.Parse([Header, "UTR-CM-508 A;52.09189;5.1174"]);

            Assert.Equal(52.09189, result.Cameras[0].Latitude);
            Assert.Equal(5.1174, result.Cameras[0].Longitude);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Theory]
    [InlineData("UTR-CM-501 Neude plein", 501)]
    [InlineData("UTR-CM-544-Janskerkhof (plein)", 544)]
    [InlineData("UTR-CM-1001 Test", 1001)]
    public void Parse_ExtractsNumberRegardlessOfSeparatorAfterIt(string name, int expectedNumber)
    {
        var result = CameraCsvParser.Parse([Header, $"{name};52.1;5.1"]);

        Assert.Equal(expectedNumber, result.Cameras[0].Number);
    }

    [Theory]
    [InlineData("UTR-CM-501 Neude;52.1")]               // too few fields
    [InlineData("No number here;52.1;5.1")]             // no camera number
    [InlineData("UTR-CM-abc Neude;52.1;5.1")]           // number is not numeric
    [InlineData("UTR-CM-501 Neude;north;5.1")]          // invalid latitude
    [InlineData("UTR-CM-501 Neude;52.1;east")]          // invalid longitude
    public void Parse_InvalidLine_IsSkippedAndReported(string line)
    {
        var result = CameraCsvParser.Parse([Header, line]);

        Assert.Empty(result.Cameras);
        var issue = Assert.Single(result.Issues);
        Assert.Equal(2, issue.LineNumber);
        Assert.Equal(line, issue.Line);
    }

    [Fact]
    public void Parse_ErrorLineInTheMiddle_KeepsGoodLinesAndReportsTheBadOne()
    {
        var result = CameraCsvParser.Parse(
        [
            Header,
            "UTR-CM-556-Laan v Puntenburg;52.086608;5.114757",
            "ERROR 1207 Could not read camera information from database",
            "UTR-CM-557-Stationsplein;52.088382;5.112054",
        ]);

        Assert.Equal([556, 557], result.Cameras.Select(c => c.Number));
        var issue = Assert.Single(result.Issues);
        Assert.Equal(3, issue.LineNumber);
    }
}