using ClosedXML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace S_Coefficient.Tests
{
    [TestClass]
    public class CalcSfactorTests
    {
        static string ExpectDir;
        static string ResultDir;

        static CalcSfactorTests()
        {
            ExpectDir = TestFiles.Combine("Expect");
            ResultDir = TestFiles.Combine("Result~");
            Directory.CreateDirectory(ResultDir);
        }

        [TestMethod]
        [DataRow("C-14")]
        public void TestPCHIP(string nuclide)
        {
            CalcSfactor CalcS = new CalcSfactor();
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
        [DataRow("C-14")]
        public void TestLinear(string nuclide)
        {
            CalcSfactor CalcS = new CalcSfactor();
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
