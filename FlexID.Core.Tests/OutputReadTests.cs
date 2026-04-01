namespace FlexID;

[TestClass]
public class OutputReadTests
{
    static readonly (OutputType type, string suffix)[] types =
    [
        (OutputType.RetentionActivity,   "Retention"),
        (OutputType.CumulativeActivity,  "Cumulative"),
        (OutputType.Dose,                "Dose"),
        (OutputType.DoseRate,            "DoseRate"),
    ];

    [TestMethod]
    [DataRow("Ba-133_ing_Insoluble")]
    [DataRow("Sr-90_ing_Other", "Y-90")]
    public void Test(string target, params string[] progeny)
    {
        var nuclide = target.Split('_')[0];

        var testDir = TestFiles.Combine("TrialCalc");
        var expectDir = Path.Combine(testDir, "Expect_OIR");

        foreach (var (type, suffix) in types)
        {
            var path = Path.Combine(expectDir, nuclide, $"{target}_{suffix}.out");
            var data = new OutputDataReader(path).Read();

            data.Blocks.Select(n => n.Header)
                .ShouldBe(type == OutputType.Dose || type == OutputType.DoseRate
                    ? [nuclide + " (Male)", nuclide + " (Female)",]
                    : [nuclide, .. progeny]);
        }
    }
}
