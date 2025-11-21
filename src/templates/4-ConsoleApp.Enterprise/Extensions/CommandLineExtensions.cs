//#if (UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser)
//#if (UseSystemCommandLine)
using System.CommandLine;
//#endif
//#if (UseCommandLineParser)
using CommandLine;
//#endif

namespace ConsoleApp.Enterprise.Extensions;

/// <summary>
/// Extension methods for command-line argument parsing.
/// </summary>
public static class CommandLineExtensions
{
//#if (UseSystemCommandLine)
    /// <summary>
    /// Parses command-line arguments using System.CommandLine.
    /// </summary>
    public static CommandLineOptions ParseCommandLine(string[] args)
    {
        var options = new CommandLineOptions();

        var rootCommand = new RootCommand("Console application with command-line arguments");

        var nameOption = new Option<string>(
            aliases: new[] { "--name", "-n" },
            description: "Your name",
            getDefaultValue: () => "World");

        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose output");

        rootCommand.AddOption(nameOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler((name, verbose) =>
        {
            options.Name = name;
            options.Verbose = verbose;
        }, nameOption, verboseOption);

        rootCommand.Invoke(args);

        return options;
    }
//#endif

//#if (UseSpectreConsole)
    /// <summary>
    /// Parses command-line arguments using Spectre.Console.
    /// Note: For complex CLI apps with Spectre.Console, consider using CommandApp pattern.
    /// This is a simplified example for basic argument parsing.
    /// </summary>
    public static CommandLineOptions ParseCommandLine(string[] args)
    {
        var options = new CommandLineOptions();

        // Simple argument parsing (Spectre.Console is better for rich console UI)
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--name" || args[i] == "-n") && i + 1 < args.Length)
            {
                options.Name = args[i + 1];
                i++;
            }
            else if (args[i] == "--verbose" || args[i] == "-v")
            {
                options.Verbose = true;
            }
        }

        return options;
    }
//#endif

//#if (UseCommandLineParser)
    /// <summary>
    /// Command-line options with CommandLineParser attributes.
    /// </summary>
    public class CommandLineParserOptions
    {
        [Option('n', "name", Required = false, HelpText = "Your name", Default = "World")]
        public string Name { get; set; } = "World";

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }

    /// <summary>
    /// Parses command-line arguments using CommandLineParser.
    /// </summary>
    public static CommandLineOptions ParseCommandLine(string[] args)
    {
        var options = new CommandLineOptions();

        Parser.Default.ParseArguments<CommandLineParserOptions>(args)
            .WithParsed(parsed =>
            {
                options.Name = parsed.Name;
                options.Verbose = parsed.Verbose;
            })
            .WithNotParsed(errors =>
            {
                // Errors are automatically displayed by CommandLineParser
                Environment.Exit(1);
            });

        return options;
    }
//#endif
}
//#endif
