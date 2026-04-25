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
