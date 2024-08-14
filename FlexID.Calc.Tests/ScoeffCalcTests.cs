using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class ScoeffCalcTests
    {
        static SAFData safdataAM;
        static SAFData safdataAF;

        static ScoeffCalcTests()
        {
            safdataAM = SAFDataReader.ReadSAF(Sex.Male);
            safdataAF = SAFDataReader.ReadSAF(Sex.Female);
        }

        [TestMethod]
        [DataRow("Ba-133")]
        [DataRow("C-14")]
        [DataRow("Ca-45")]
        [DataRow("Cs-134")]
        [DataRow("Cs-137")]
        [DataRow("Ba-137m")]
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
        [DataRow("Y-90")]
        [DataRow("Tc-99")]
        [DataRow("Zn-65")]
        public void TestPCHIP(string nuclide)
        {
            var expectDir = TestFiles.Combine("SCoeffCalc", "ExpectPCHIP");
            var resultDir = TestFiles.Combine("SCoeffCalc", "ResultPCHIP~");
            Directory.CreateDirectory(resultDir);

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var safdata = sex == Sex.Male ? safdataAM : safdataAF;
                var calcS = new CalcScoeff(safdata);
                calcS.InterpolationMethod = "PCHIP";

                calcS.CalcS(nuclide);

                var target = $@"{nuclide}_{(sex == Sex.Male ? "AM" : "AF")}";
                var actualFilePath = Path.Combine(resultDir, target + ".txt");
                calcS.WriteOutTotalResult(actualFilePath);
            }

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var target = $@"{nuclide}_{(sex == Sex.Male ? "AM" : "AF")}";
                var expectFilePath = Path.Combine(expectDir, target + ".txt");
                var actualFilePath = Path.Combine(resultDir, target + ".txt");
                CollectionAssert.AreEqual(
                    File.ReadAllLines(expectFilePath),
                    File.ReadAllLines(actualFilePath));
            }
        }

        [TestMethod]
        [DataRow("Ba-133")]
        [DataRow("C-14")]
        [DataRow("Ca-45")]
        [DataRow("Cs-134")]
        [DataRow("Cs-137")]
        [DataRow("Ba-137m")]
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
        [DataRow("Y-90")]
        [DataRow("Tc-99")]
        [DataRow("Zn-65")]
        public void TestLinear(string nuclide)
        {
            var expectDir = TestFiles.Combine("SCoeffCalc", "ExpectLinear");
            var resultDir = TestFiles.Combine("SCoeffCalc", "ResultLinear~");
            Directory.CreateDirectory(resultDir);

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var safdata = sex == Sex.Male ? safdataAM : safdataAF;
                var calcS = new CalcScoeff(safdata);
                calcS.InterpolationMethod = "線形補間";

                calcS.CalcS(nuclide);

                var target = $@"{nuclide}_{(sex == Sex.Male ? "AM" : "AF")}";
                var actualFilePath = Path.Combine(resultDir, target + ".txt");
                calcS.WriteOutTotalResult(actualFilePath);
            }

            foreach (var sex in new[] { Sex.Male, Sex.Female })
            {
                var target = $@"{nuclide}_{(sex == Sex.Male ? "AM" : "AF")}";
                var expectFilePath = Path.Combine(expectDir, target + ".txt");
                var actualFilePath = Path.Combine(resultDir, target + ".txt");
                CollectionAssert.AreEqual(
                    File.ReadAllLines(expectFilePath),
                    File.ReadAllLines(actualFilePath));
            }
        }
    }
}
