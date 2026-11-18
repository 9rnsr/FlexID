using System.Text;

namespace FlexID;

class TestFiles
{
    static TestFiles()
    {
        var testfilesPath = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        testfilesDir = File.ReadLines(testfilesPath).First();
    }

    private static readonly string testfilesDir;

    public static string Combine(params string[] paths)
    {
        return Path.Combine(testfilesDir, Path.Combine(paths));
    }

    public static string ReplaceVar(string str)
    {
        str = str.Replace("$(TestFiles)", testfilesDir);
        str = str.Replace("$(ResourceFiles)", AppResource.BaseDir);
        return str;
    }
}

static class InputReadTestHelpers
{
    public static InputDataReader_OIR CreateReader(string[] inputLines)
    {
        var inputFileBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, inputLines));

        var stream = new MemoryStream(inputFileBytes);
        var reader = new StreamReader(stream);
        return new InputDataReader_OIR(reader);
    }

    public static InputDataReader_OIR CreateReader(string inputText)
    {
        var inputFileBytes = Encoding.UTF8.GetBytes(inputText);

        var stream = new MemoryStream(inputFileBytes);
        var reader = new StreamReader(stream);
        return new InputDataReader_OIR(reader);
    }

    public static IEnumerable<string> GetErrorLines(this InputErrors errors)
    {
        try { errors.RaiseIfAny(); }
        catch (InputErrorsException e) { return e.ErrorLines; }
        return Enumerable.Empty<string>();
    }
}
