using System.CommandLine;
using System.Globalization;

namespace FlexID;

internal partial class Program
{
    static async Task<int> Main(string[] args)
    {
        // OSの言語設定を無視して、英語メッセージを表示する。
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

        RootCommand rootCommand = new("CLI interface for FlexID (Flexible code for Internal Dosimetry)");
        rootCommand.Subcommands.Add(Program_Run.Command);
        rootCommand.Subcommands.Add(Program_Gen.Command);

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
