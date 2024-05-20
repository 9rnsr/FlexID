using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class TrialCalcTests
    {
        private const string TestDir = @"TestFiles/TrialCalc";

        [Theory]
        [InlineData(@"Ba-133_ing-Insoluble")]
        [InlineData(@"Ba-133_ing-Soluble")]
        [InlineData(@"Ba-133_inh-TypeF")]
        [InlineData(@"Ba-133_inh-TypeM")]
        [InlineData(@"Ba-133_inh-TypeS")]
        [InlineData(@"C-14_ing")]
        [InlineData(@"C-14_inh-TypeF")]
        [InlineData(@"C-14_inh-TypeF-Barium")]
        [InlineData(@"C-14_inh-TypeF-Gas")]
        [InlineData(@"C-14_inh-TypeM")]
        [InlineData(@"C-14_inh-TypeS")]
        [InlineData(@"Ca-45_ing")]
        [InlineData(@"Ca-45_inh-TypeF")]
        [InlineData(@"Ca-45_inh-TypeM")]
        [InlineData(@"Ca-45_inh-TypeS")]
        [InlineData(@"Cs-134_ing-Insoluble")]
        [InlineData(@"Cs-134_ing-Unspecified")]
        [InlineData(@"Cs-134_inh-TypeF")]
        [InlineData(@"Cs-134_inh-TypeM")]
        [InlineData(@"Cs-134_inh-TypeS")]
        [InlineData(@"Cs-137_ing-Insoluble")]
        [InlineData(@"Cs-137_ing-Unspecified")]
        [InlineData(@"Cs-137_inh-TypeF")]
        [InlineData(@"Cs-137_inh-TypeM")]
        [InlineData(@"Cs-137_inh-TypeS")]
        [InlineData(@"Fe-59_ing")]
        [InlineData(@"Fe-59_inh-TypeF")]
        [InlineData(@"Fe-59_inh-TypeM")]
        [InlineData(@"Fe-59_inh-TypeS")]
        [InlineData(@"H-3_ing-Insoluble")]
        [InlineData(@"H-3_ing-Organic")]
        [InlineData(@"H-3_ing-Soluble")]
        [InlineData(@"H-3_inh-TypeF-Gas")]
        [InlineData(@"H-3_inh-TypeF-Organic")]
        [InlineData(@"H-3_inh-TypeF-Tritide")]
        [InlineData(@"H-3_inh-TypeM")]
        [InlineData(@"H-3_inh-TypeS")]
        [InlineData(@"I-129_ing")]
        [InlineData(@"I-129_inh-TypeF")]
        [InlineData(@"I-129_inh-TypeM")]
        [InlineData(@"I-129_inh-TypeS")]
        [InlineData(@"Pu-238_ing-Insoluble")]
        [InlineData(@"Pu-238_ing-Unidentified")]
        [InlineData(@"Pu-238_inh-TypeF")]
        [InlineData(@"Pu-238_inh-TypeM")]
        [InlineData(@"Pu-238_inh-TypeS")]
        [InlineData(@"Pu-239_ing-Insoluble")]
        [InlineData(@"Pu-239_ing-Unidentified")]
        [InlineData(@"Pu-239_inh-TypeF")]
        [InlineData(@"Pu-239_inh-TypeM")]
        [InlineData(@"Pu-239_inh-TypeS")]
        [InlineData(@"Pu-239_inj")]
        [InlineData(@"Pu-240_ing-Insoluble")]
        [InlineData(@"Pu-240_ing-Unidentified")]
        [InlineData(@"Pu-240_inh-TypeF")]
        [InlineData(@"Pu-240_inh-TypeM")]
        [InlineData(@"Pu-240_inh-TypeS")]
        [InlineData(@"Pu-241_ing-Insolube")]
        [InlineData(@"Pu-241_ing-Unidentified")]
        [InlineData(@"Pu-241_inh-TypeF")]
        [InlineData(@"Pu-241_inh-TypeM")]
        [InlineData(@"Pu-241_inh-TypeS")]
        [InlineData(@"Pu-242_ing-Insoluble")]
        [InlineData(@"Pu-242_ing-Unidentified")]
        [InlineData(@"Pu-242_inh-TypeF")]
        [InlineData(@"Pu-242_inh-TypeM")]
        [InlineData(@"Pu-242_inh-TypeS")]
        [InlineData(@"Ra-223_inh-TypeF")]
        [InlineData(@"Ra-226_ing")]
        [InlineData(@"Ra-226_inh-TypeF")]
        [InlineData(@"Ra-226_inh-TypeM")]
        [InlineData(@"Ra-226_inh-TypeS")]
        [InlineData(@"Sr-90_ing-Other")]
        [InlineData(@"Sr-90_ing-Titanate")]
        [InlineData(@"Sr-90_inh-TypeF")]
        [InlineData(@"Sr-90_inh-TypeM")]
        [InlineData(@"Sr-90_inh-TypeS")]
        [InlineData(@"Tc-99_ing")]
        [InlineData(@"Tc-99_inh-TypeF")]
        [InlineData(@"Tc-99_inh-TypeM")]
        [InlineData(@"Tc-99_inh-TypeS")]
        [InlineData(@"Zn-65_ing")]
        [InlineData(@"Zn-65_inh-TypeF")]
        [InlineData(@"Zn-65_inh-TypeM")]
        [InlineData(@"Zn-65_inh-TypeS")]
        public void Test_OIR(string target)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "OIR", nuclide, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_OIR", nuclide);
            var resultDir = Path.Combine(TestDir, "Result_OIR~", nuclide);
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\time.dat";
            var oTimeMeshFile = @"TestFiles\TrialCalc\out-time.dat";

            var commitmentPeriod = "50years";

            var main = new MainRoutine_OIR();
            main.InputPath        /**/= inputPath;
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;
            main.CalcProgeny      /**/= true;

            main.Main();

            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Cumulative.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Cumulative.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Dose.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Dose.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_DoseRate.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_DoseRate.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Retention.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Retention.out")));
        }

        [Theory]
        [InlineData(@"Sr-90_ing", "3months old")]
        [InlineData(@"Sr-90_ing", "1years old")]
        [InlineData(@"Sr-90_ing", "5years old")]
        [InlineData(@"Sr-90_ing", "10years old")]
        [InlineData(@"Sr-90_ing", "15years old")]
        [InlineData(@"Sr-90_ing", "adult")]
        public void Test_EIR(string target, string exposureAge)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "EIR", nuclide, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_EIR", nuclide, exposureAge.Replace(' ', '_'));
            var resultDir = Path.Combine(TestDir, "Result_EIR~", nuclide, exposureAge.Replace(' ', '_'));
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\time.dat";
            var oTimeMeshFile = @"TestFiles\TrialCalc\out-time.dat";

            var commitmentPeriod =
                exposureAge == "3months old" /**/? "25450days" : // 70years - 100days = 25550days - 100days
                exposureAge == "1years old"  /**/? "69years" :   // 70years -  1years
                exposureAge == "5years old"  /**/? "65years" :   // 70years -  5years
                exposureAge == "10years old" /**/? "60years" :   // 70years - 10years
                exposureAge == "15years old" /**/? "55years" :   // 70years - 15years
                exposureAge == "adult"       /**/? "50years" :   // 75years - 25years
                throw new NotSupportedException();

            var main = new MainRoutine_EIR();
            main.InputPath        /**/= inputPath;
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;
            main.CalcProgeny      /**/= true;
            main.ExposureAge      /**/= exposureAge;

            main.Main();

            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Cumulative.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Cumulative.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Dose.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Dose.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_DoseRate.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_DoseRate.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(expectDir, target + "_Retention.out")),
                File.ReadLines(Path.Combine(resultDir, target + "_Retention.out")));
        }
    }
}
