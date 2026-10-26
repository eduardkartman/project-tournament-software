using System.Globalization;
using CameraExplorer.Cli;
using CameraExplorer.Core;

namespace CameraExplorer.Tests;

public class CliTests
{
    [Theory]
    [InlineData("--name")]
    [InlineData("-n")]
    public void Parse_NameOption_ReturnsName(string flag)
    {
        var options = ArgumentParser.Parse([flag, "Neude"]);

        Assert.Equal("Neude", options.Name);
        Assert.False(options.ShowHelp);
    }

    [Fact]
    public void Parse_NoArguments_ReturnsNullName()
    {
        Assert.Null(ArgumentParser.Parse([]).Name);
    }

    [Fact]
    public void Parse_Help_SetsShowHelp()
    {
        Assert.True(ArgumentParser.Parse(["--help"]).ShowHelp);
    }

    [Fact]
    public void Parse_NameWithoutValue_Throws()
    {
        Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(["--name"]));
    }

    [Fact]
    public void Parse_UnknownArgument_Throws()
    {
        Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(["--bogus"]));
    }

    [Fact]
    public void Format_ProducesPipeSeparatedLine()
    {
        var camera = new Camera(501, "UTR-CM-501 Neude rijbaan voor Postkantoor", 52.093421, 5.118278);

        Assert.Equal(
            "501 | UTR-CM-501 Neude rijbaan voor Postkantoor | 52.093421 | 5.118278",
            CameraFormatter.Format(camera));
    }

    [Fact]
    public void Format_IgnoresCurrentCulture()
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ro-RO");
        try
        {
            var line = CameraFormatter.Format(new Camera(508, "UTR-CM-508 A", 52.09189, 5.1174));

            Assert.Equal("508 | UTR-CM-508 A | 52.09189 | 5.1174", line);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}