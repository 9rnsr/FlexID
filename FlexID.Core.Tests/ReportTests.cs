namespace FlexID;

[TestClass]
public class ReportTests
{
    [TestMethod]
    public void TestWithCompare()
    {
        var dir = TestFiles.Combine("ReportGen");

        var expectDir = Path.Combine(AppResource.BaseDir, @"expect");
        var expects = Directory.EnumerateFiles(expectDir, "*.dat", SearchOption.AllDirectories)
            .Select(path => KeyValuePair.Create(Path.GetFileNameWithoutExtension(path), path))
            .Where(expect => expect.Key.Contains('_'))  // Leave the expect retention files only (e.g. 'H-3_Injection.dat').
            .ToDictionary();

        var resultDir = Path.Combine(dir, "~Result");
        Directory.CreateDirectory(resultDir);

        var reports = Directory.EnumerateFiles(dir, "*.log").Select(path =>
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var outputPath = Path.Combine(dir, name);
            var compare = (Name: name, Path: expects[name]);
            var report = new ReportData(null, outputPath, compare);
            report.LoadResult();
            return report;
        }).ToArray();

        var report = reports[0];
        ReportGenerator.WriteReport(Path.Combine(resultDir, report.OutputName + "_compare.xlsx"), report);

        var sortedReports = reports.OrderBy(r => r.SortKey).ToArray();

        var summaryFile = Path.Combine(resultDir, "summary_compare.xlsx");
        ReportGenerator.WriteSummary(summaryFile, sortedReports);
    }

    [TestMethod]
    public void TestWithoutCompare()
    {
        var dir = TestFiles.Combine("ReportGen");

        var resultDir = Path.Combine(dir, "~Result");
        Directory.CreateDirectory(resultDir);

        var reports = Directory.EnumerateFiles(dir, "*.log").Select(path =>
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var outputPath = Path.Combine(dir, name);
            var report = new ReportData(null, outputPath, compare: null);
            report.LoadResult();
            return report;
        }).ToArray();

        var report = reports[0];
        ReportGenerator.WriteReport(Path.Combine(resultDir, report.OutputName + "_output.xlsx"), report);

        var sortedReports = reports.OrderBy(r => r.SortKey).ToArray();

        var summaryFile = Path.Combine(resultDir, "summary_output.xlsx");
        ReportGenerator.WriteSummary(summaryFile, sortedReports);
    }
}
