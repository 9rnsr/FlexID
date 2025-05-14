using System.IO;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class ScoeffCalcTests
    {
        static SAFData safdataAM;
        static SAFData safdataAF;

        static ScoeffCalcTests()
        {
            safdataAM = SAFDataReader.ReadSAF(Sex.Male);
            safdataAF = SAFDataReader.ReadSAF(Sex.Female);
        }

        [Theory]
        [InlineData("Ba-133")]
        [InlineData("C-14")]
        [InlineData("Ca-45")]
        [InlineData("Cs-134")]
        [InlineData("Cs-137")]
        [InlineData("Ba-137m")]
        [InlineData("Fe-59")]
        [InlineData("H-3")]
        [InlineData("I-129")]
        [InlineData("Pu-238")]
        [InlineData("Pu-239")]
        [InlineData("Pu-240")]
        [InlineData("Pu-241")]
        [InlineData("Pu-242")]
        [InlineData("Ra-223")]
        [InlineData("Ra-226")]
        [InlineData("Sr-90")]
        [InlineData("Y-90")]
        [InlineData("Tc-99")]
        [InlineData("Zn-65")]
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
                Assert.Equal(
                    File.ReadAllLines(expectFilePath),
                    File.ReadAllLines(actualFilePath));
            }
        }

        [Theory]
        [InlineData("Ba-133")]
        [InlineData("C-14")]
        [InlineData("Ca-45")]
        [InlineData("Cs-134")]
        [InlineData("Cs-137")]
        [InlineData("Ba-137m")]
        [InlineData("Fe-59")]
        [InlineData("H-3")]
        [InlineData("I-129")]
        [InlineData("Pu-238")]
        [InlineData("Pu-239")]
        [InlineData("Pu-240")]
        [InlineData("Pu-241")]
        [InlineData("Pu-242")]
        [InlineData("Ra-223")]
        [InlineData("Ra-226")]
        [InlineData("Sr-90")]
        [InlineData("Y-90")]
        [InlineData("Tc-99")]
        [InlineData("Zn-65")]
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
                Assert.Equal(
                    File.ReadAllLines(expectFilePath),
                    File.ReadAllLines(actualFilePath));
            }
        }
    }
}
