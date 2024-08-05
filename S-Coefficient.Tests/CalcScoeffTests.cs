using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace S_Coefficient.Tests
{
    [TestClass]
    public class CalcScoeffTests
    {
        static string ExpectDir;
        static string ResultDir;

        static CalcScoeffTests()
        {
            ExpectDir = TestFiles.Combine("Expect");
            ResultDir = TestFiles.Combine("Result~");
            Directory.CreateDirectory(ResultDir);
        }

        [TestMethod]
        [DataRow("Ba-133")]
        [DataRow("C-14")]
        [DataRow("Ca-45")]
        [DataRow("Cs-134")]
        [DataRow("Cs-137")]
        [DataRow("Fe-59")]
        [DataRow("H-3")]
        [DataRow("I-129")]
        [DataRow("Pu-238")]
        [DataRow("Pu-239")]
        [DataRow("Pu-240")]
        [DataRow("Pu-241")]
        [DataRow("Pu-242")]
        [DataRow("Ra-223")]
        [DataRow("Ra-226")]
        [DataRow("Sr-90")]
        [DataRow("Tc-99")]
        [DataRow("Zn-65")]
        public void TestPCHIP(string nuclide)
        {
            var CalcS = new CalcScoeff();
            CalcS.InterpolationMethod = "PCHIP";

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var target = $@"PCHIP_{nuclide}_Adult{sex}";

                var safdata = DataReader.ReadSAF(sex);
                Assert.IsTrue(safdata.Completion);

                var output = new Output();
                output.OutputExcelFilePath = Path.Combine(ResultDir, target + ".xlsx");
                output.OutputTextFilePath = Path.Combine(ResultDir, target + ".txt");

                CalcS.CalcS(safdata, nuclide, output);

                CollectionAssert.AreEqual(
                    File.ReadAllLines(Path.Combine(ExpectDir, target + ".txt")),
                    File.ReadAllLines(Path.Combine(ResultDir, target + ".txt")));
            }
        }

        [TestMethod]
        [DataRow("Ba-133")]
        [DataRow("C-14")]
        [DataRow("Ca-45")]
        [DataRow("Cs-134")]
        [DataRow("Cs-137")]
        [DataRow("Fe-59")]
        [DataRow("H-3")]
        [DataRow("I-129")]
        [DataRow("Pu-238")]
        [DataRow("Pu-239")]
        [DataRow("Pu-240")]
        [DataRow("Pu-241")]
        [DataRow("Pu-242")]
        [DataRow("Ra-223")]
        [DataRow("Ra-226")]
        [DataRow("Sr-90")]
        [DataRow("Tc-99")]
        [DataRow("Zn-65")]
        public void TestLinear(string nuclide)
        {
            var CalcS = new CalcScoeff();
            CalcS.InterpolationMethod = "線形補間";

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var target = $@"Linear_{nuclide}_Adult{sex}";

                var safdata = DataReader.ReadSAF(sex);
                Assert.IsTrue(safdata.Completion);

                var output = new Output();
                output.OutputExcelFilePath = Path.Combine(ResultDir, target + ".xlsx");
                output.OutputTextFilePath = Path.Combine(ResultDir, target + ".txt");

                CalcS.CalcS(safdata, nuclide, output);

                CollectionAssert.AreEqual(
                    File.ReadAllLines(Path.Combine(ExpectDir, target + ".txt")),
                    File.ReadAllLines(Path.Combine(ResultDir, target + ".txt")));
            }
        }
    }
}
