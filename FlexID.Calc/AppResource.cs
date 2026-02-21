namespace FlexID.Calc;

public static class AppResource
{
    private const string ResourceFilesName = "ResourceFiles";

    public static string ProcessDir { get; }

    public static string BaseDir { get; }

    static AppResource()
    {
        ProcessDir = Path.GetDirectoryName(Environment.ProcessPath)
                  ?? Directory.GetCurrentDirectory();

        var resourceFilesPath = Path.Combine(ProcessDir, ResourceFilesName);
        if (File.Exists(resourceFilesPath))
            BaseDir = File.ReadLines(resourceFilesPath).First();
        else
            BaseDir = ProcessDir;
    }
}
