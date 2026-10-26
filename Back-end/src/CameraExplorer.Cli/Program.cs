using System.Text;
using CameraExplorer.Cli;
using CameraExplorer.Core;
using CameraExplorer.Infrastructure;

Console.OutputEncoding = Encoding.UTF8;

try
{
    var options = ArgumentParser.Parse(args);

    if (options.ShowHelp)
    {
        PrintUsage();
        return 0;
    }

    if (options.Name is null)
    {
        Console.Error.WriteLine("The --name argument is required.");
        PrintUsage();
        return 2;
    }

    var csvPath = Path.Combine(AppContext.BaseDirectory, "data", "cameras-defb.csv");
    var service = new CameraService(new CsvCameraRepository(csvPath));

    foreach (var issue in service.GetLoadIssues())
        Console.Error.WriteLine($"Warning: skipped line {issue.LineNumber} ('{issue.Line}'): {issue.Reason}.");

    var matches = service.Search(options.Name);

    foreach (var camera in matches)
        Console.WriteLine(CameraFormatter.Format(camera));

    if (matches.Count == 0)
        Console.Error.WriteLine($"No cameras found matching '{options.Name}'.");

    return 0;
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    PrintUsage();
    return 2;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Could not load camera data: {ex.Message}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("Usage: Search --name <text>");
    Console.WriteLine("  -n, --name <text>   Show cameras whose name contains <text> (case-insensitive)");
    Console.WriteLine("  -h, --help          Show this help");
}