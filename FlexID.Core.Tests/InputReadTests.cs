namespace FlexID;

[TestClass]
public class InputReadTests
{
    [TestMethod]
    [DynamicData(nameof(GetTargets))]
    public void Test_OIR(string inputPath)
    {
        var target = Path.GetFileNameWithoutExtension(inputPath);
        var nuclide = target.Split('_')[0];

        var data = new InputDataReader_OIR(inputPath).Read();
        data.ShouldNotBeNull();
    }

    public static IEnumerable<object[]> GetTargets()
    {
        var inputDir = Path.Combine(AppResource.BaseDir, @"inp\OIR");
        return Directory.EnumerateFiles(inputDir, "*.inp", SearchOption.AllDirectories)
            .Select(path => new object[] { path });
    }

    [TestMethod]
    [DataRow("Sr-90_ing")]
    public void Test_EIR(string target)
    {
        var nuclide = target.Split('_')[0];
        var inputPath = Path.Combine(AppResource.BaseDir, @"inp\EIR", nuclide, target + ".inp");

        var data = new InputDataReader_EIR(inputPath).Read();
        data.ShouldNotBeNull();
    }
}
