using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class OutputReadTests
    {
        static readonly (OutputType type, string suffix)[] types = new[]
        {
            (OutputType.RetentionActivity,   "Retention"),
            (OutputType.CumulativeActivity,  "Cumulative"),
            (OutputType.Dose,                "Dose"),
            (OutputType.DoseRate,            "DoseRate"),
        };

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

                Assert.AreEqual(nuclide, data.Nuclides[0].Nuclide);
                if (type == OutputType.Dose || type == OutputType.DoseRate)
                {
                    Assert.AreEqual(1, data.Nuclides.Length);
                }
                else
                {
                    Assert.AreEqual(1 + progeny.Length, data.Nuclides.Length);
                    Assert.AreEqual(nuclide, data.Nuclides[0].Nuclide);
                    CollectionAssert.AreEqual(progeny, data.Nuclides.Skip(1).Select(n => n.Nuclide).ToArray());
                }
            }
        }
    }
}
