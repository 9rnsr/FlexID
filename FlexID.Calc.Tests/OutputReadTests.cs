using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class OutputReadTests
    {
        [TestMethod]
        public void Test()
        {
            var TestDir = TestFiles.Combine("TrialCalc");
            var expectDir = Path.Combine(TestDir, "Expect_OIR");

            var nuclide = "Ba-133";
            var target = "Ba-133_ing-Insoluble";

            var cumu = Path.Combine(expectDir, nuclide, target + "_Cumulative.out");
            var data = new OutputDataReader(cumu).Read();

            Assert.AreEqual(1, data.Nuclides.Length);
            Assert.AreEqual("Ba-133", data.Nuclides[0].Nuclide);

            //File.ReadAllLines(cumu);
            //File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out"));
            //File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out"));
            //File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out"));
        }

        [TestMethod]
        public void TestWithProgeny()
        {
            var TestDir = TestFiles.Combine("TrialCalc");
            var expectDir = Path.Combine(TestDir, "Expect_OIR");

            var nuclide = "Sr-90";
            var target = "Sr-90_ing-Other";

            var cumu = Path.Combine(expectDir, nuclide, target + "_Cumulative.out");
            var data = new OutputDataReader(cumu).Read();

            Assert.AreEqual(2, data.Nuclides.Length);
            Assert.AreEqual("Sr-90", data.Nuclides[0].Nuclide);
            Assert.AreEqual("Y-90", data.Nuclides[1].Nuclide);

            //File.ReadAllLines(cumu);
            //File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out"));
            //File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out"));
            //File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out"));

        }
    }
}
