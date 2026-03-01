using System.CommandLine;

namespace FlexID;

internal partial class Program
{
    static async Task<int> Main(string[] args)
    {
        RootCommand rootCommand = new("CLI interface for FlexID (Flexible code for Internal Dosimetry)");
        rootCommand.Subcommands.Add(Program_Run.Command);
        rootCommand.Subcommands.Add(Program_Gen.Command);

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
