namespace FlexID.Calc.Tests;

using System.Reflection;

class TestFiles
{
    static TestFiles()
    {
        var assembly = Assembly.GetAssembly(typeof(TestFiles));
        var assemblyDir = Path.GetDirectoryName(assembly.Location);

        testFilesDir = File.ReadLines(Path.Combine(assemblyDir, "TestFiles")).First();
    }

    private static readonly string testFilesDir;

    public static string Combine(params string[] paths)
    {
        return Path.Combine(testFilesDir, Path.Combine(paths));
    }

    public static string ReplaceVar(string str, string pattern = "$(TestFiles)")
    {
        return str.Replace(pattern, testFilesDir);
    }
}
