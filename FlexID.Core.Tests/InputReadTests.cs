namespace FlexID;

[TestClass]
public class InputReadTests
{
    private static readonly string InputOirDir;
    private static readonly string InputEirDir;

    private static readonly string[] RetentionExpects;

    static InputReadTests()
    {
        InputOirDir = Path.Combine(AppResource.BaseDir, @"inp\OIR");
        InputEirDir = Path.Combine(AppResource.BaseDir, @"inp\EIR");

        var expectDir = Path.Combine(AppResource.BaseDir, @"expect");
        RetentionExpects = Directory.EnumerateFiles(expectDir, "*.dat", SearchOption.AllDirectories)
            .Select(path =>
            {
                var nuclide = Path.GetFileName(Path.GetDirectoryName(path));
                var datfile = Path.GetFileName(path);
                return Path.Combine(nuclide, datfile);
            }).ToArray();
    }

    [TestMethod]
    [DynamicData(nameof(GetOirInuts))]
    public void Test_OIR(string inputPath)
    {
        inputPath = Path.Combine(InputOirDir, inputPath);

        var target = Path.GetFileNameWithoutExtension(inputPath);
        var nuclide = target.Split('_')[0];

        var data = new InputDataReader_OIR(inputPath).Read();
        data.ShouldNotBeNull();

        // インプットファイル名から取得した核種が、親核種として設定されているか確認。
        data.Nuclides.First().Name.ShouldBe(nuclide);

        var expectDat = Path.Combine(nuclide, target + ".dat");
        RetentionExpects.ShouldContain(expectDat, $"Expect dat file {expectDat} not found for target {target}");
    }

    public static IEnumerable<object[]> GetOirInuts()
    {
        return Directory.EnumerateFiles(InputOirDir, "*.inp", SearchOption.AllDirectories)
            .Select(path => new object[] { Path.GetRelativePath(InputOirDir, path) });
    }

    [TestMethod]
    [DynamicData(nameof(GetEirInuts))]
    public void Test_EIR(string inputPath)
    {
        inputPath = Path.Combine(InputEirDir, inputPath);

        var target = Path.GetFileNameWithoutExtension(inputPath);
        var nuclide = target.Split('_')[0];

        var data = new InputDataReader_EIR(inputPath).Read();
        data.ShouldNotBeNull();

        // インプットファイル名から取得した核種が、親核種として設定されているか確認。
        data[0].Nuclides.First().Name.ShouldBe(nuclide);
    }

    public static IEnumerable<object[]> GetEirInuts()
    {
        return Directory.EnumerateFiles(InputEirDir, "*.inp", SearchOption.AllDirectories)
            .Select(path => new object[] { Path.GetRelativePath(InputEirDir, path) });
    }
}
