namespace FlexID.Calc.Tests;

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
    [DataRow("Ba-133_ing-Insoluble")]
    [DataRow("Sr-90_ing-Other", "Y-90")]
    public void Test(string target, params string[] progeny)
    {
        var nuclide = target.Split('_')[0];

        var TestDir = TestFiles.Combine("TrialCalc");
        var expectDir = Path.Combine(TestDir, "Expect_OIR");

        foreach (var (type, suffix) in types)
        {
            var path = Path.Combine(expectDir, nuclide, $"{target}_{suffix}.out");
            var data = new OutputDataReader(path).Read();

            if (type == OutputType.Dose || type == OutputType.DoseRate)
            {
                Assert.AreEqual(2, data.Blocks.Length);
                Assert.AreEqual(nuclide + " (Male)", data.Blocks[0].Header);
                Assert.AreEqual(nuclide + " (Female)", data.Blocks[1].Header);
            }
            else
            {
                Assert.AreEqual(nuclide, data.Blocks[0].Header);
                Assert.AreEqual(1 + progeny.Length, data.Blocks.Length);
                Assert.AreEqual(nuclide, data.Blocks[0].Header);
                CollectionAssert.AreEqual(progeny, data.Blocks.Skip(1).Select(n => n.Header).ToArray());
            }
        }
    }
}
