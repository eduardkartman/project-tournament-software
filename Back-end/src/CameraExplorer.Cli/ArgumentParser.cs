namespace CameraExplorer.Cli;

public sealed record CliOptions(string? Name, bool ShowHelp);

public static class ArgumentParser
{
    /// <summary>Parses command-line arguments. Throws ArgumentException on invalid input.</summary>
    public static CliOptions Parse(string[] args)
    {
        string? name = null;
        var showHelp = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--name" or "-n":
                    if (i + 1 >= args.Length)
                        throw new ArgumentException("Missing value for --name.");
                    name = args[++i];
                    break;

                case "--help" or "-h":
                    showHelp = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument '{args[i]}'.");
            }
        }

        return new CliOptions(name, showHelp);
    }
}