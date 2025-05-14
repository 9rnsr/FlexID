using System.IO;
using System.Linq;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class OutputReadTests
    {
        static readonly (OutputType type, string suffix)[] types = new[]
        {
            (OutputType.RetentionActivity,   "Retention"),
            (OutputType.CumulativeActivity,  "Cumulative"),
            (OutputType.Dose,                "Dose"),
            (OutputType.DoseRate,            "DoseRate"),
        };

        [Theory]
        [InlineData("Ba-133_ing-Insoluble")]
        [InlineData("Sr-90_ing-Other", "Y-90")]
        public void Test(string target, params string[] progeny)
        {
            var nuclide = target.Split('_')[0];

            var TestDir = TestFiles.Combine("TrialCalc");
            var expectDir = Path.Combine(TestDir, "Expect_OIR");

            foreach (var (type, suffix) in types)
            {
                var path = Path.Combine(expectDir, nuclide, $"{target}_{suffix}.out");
                var data = new OutputDataReader(path).Read();

                Assert.Equal(nuclide, data.Nuclides[0].Nuclide);
                if (type == OutputType.Dose || type == OutputType.DoseRate)
                {
                    Assert.Single(data.Nuclides);
                }
                else
                {
                    Assert.Equal(1 + progeny.Length, data.Nuclides.Length);
                    Assert.Equal(nuclide, data.Nuclides[0].Nuclide);
                    Assert.Equal(progeny, data.Nuclides.Skip(1).Select(n => n.Nuclide).ToArray());
                }
            }
        }
    }
}
