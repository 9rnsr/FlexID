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
        [InlineData(@"inp/OIR/Ba-133/Ba-133_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Ba-133/Ba-133_ing-Soluble.inp")]
        [InlineData(@"inp/OIR/Ba-133/Ba-133_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Ba-133/Ba-133_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Ba-133/Ba-133_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_ing.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_inh-TypeF-Barium.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_inh-TypeF-Gas.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/C-14/C-14_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Ca-45/Ca-45_ing.inp")]
        [InlineData(@"inp/OIR/Ca-45/Ca-45_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Ca-45/Ca-45_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Ca-45/Ca-45_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Cs-134/Cs-134_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Cs-134/Cs-134_ing-Unspecified.inp")]
        [InlineData(@"inp/OIR/Cs-134/Cs-134_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Cs-134/Cs-134_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Cs-134/Cs-134_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Cs-137/Cs-137_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Cs-137/Cs-137_ing-Unspecified.inp")]
        [InlineData(@"inp/OIR/Cs-137/Cs-137_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Cs-137/Cs-137_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Cs-137/Cs-137_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Fe-59/Fe-59_ing.inp")]
        [InlineData(@"inp/OIR/Fe-59/Fe-59_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Fe-59/Fe-59_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Fe-59/Fe-59_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_ing-Organic.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_ing-Soluble.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_inh-TypeF-Gas.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_inh-TypeF-Organic.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_inh-TypeF-Tritide.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/H-3/H-3_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/I-129/I-129_ing.inp")]
        [InlineData(@"inp/OIR/I-129/I-129_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/I-129/I-129_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/I-129/I-129_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Pu-238/Pu-238_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Pu-238/Pu-238_ing-Unidentified.inp")]
        [InlineData(@"inp/OIR/Pu-238/Pu-238_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Pu-238/Pu-238_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Pu-238/Pu-238_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_ing-Unidentified.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Pu-239/Pu-239_inj.inp")]
        [InlineData(@"inp/OIR/Pu-240/Pu-240_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Pu-240/Pu-240_ing-Unidentified.inp")]
        [InlineData(@"inp/OIR/Pu-240/Pu-240_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Pu-240/Pu-240_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Pu-240/Pu-240_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Pu-241/Pu-241_ing-Insolube.inp")]
        [InlineData(@"inp/OIR/Pu-241/Pu-241_ing-Unidentified.inp")]
        [InlineData(@"inp/OIR/Pu-241/Pu-241_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Pu-241/Pu-241_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Pu-241/Pu-241_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Pu-242/Pu-242_ing-Insoluble.inp")]
        [InlineData(@"inp/OIR/Pu-242/Pu-242_ing-Unidentified.inp")]
        [InlineData(@"inp/OIR/Pu-242/Pu-242_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Pu-242/Pu-242_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Pu-242/Pu-242_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Ra-223/Ra-223_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Ra-226/Ra-226_ing.inp")]
        [InlineData(@"inp/OIR/Ra-226/Ra-226_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Ra-226/Ra-226_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Ra-226/Ra-226_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Sr-90/Sr-90_ing-Other.inp")]
        [InlineData(@"inp/OIR/Sr-90/Sr-90_ing-Titanate.inp")]
        [InlineData(@"inp/OIR/Sr-90/Sr-90_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Sr-90/Sr-90_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Sr-90/Sr-90_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Tc-99/Tc-99_ing.inp")]
        [InlineData(@"inp/OIR/Tc-99/Tc-99_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Tc-99/Tc-99_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Tc-99/Tc-99_inh-TypeS.inp")]
        [InlineData(@"inp/OIR/Zn-65/Zn-65_ing.inp")]
        [InlineData(@"inp/OIR/Zn-65/Zn-65_inh-TypeF.inp")]
        [InlineData(@"inp/OIR/Zn-65/Zn-65_inh-TypeM.inp")]
        [InlineData(@"inp/OIR/Zn-65/Zn-65_inh-TypeS.inp")]
        public void Test_OIR(string inputPath)
        {
            var target = Path.GetFileNameWithoutExtension(inputPath);
            var nuclide = target.Split('_')[0];

            var expectDir = Path.Combine(TestDir, "Expect_OIR", nuclide);
            var resultDir = Path.Combine(TestDir, "Result_OIR~", nuclide);
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\time.dat";
            var oTimeMeshFile = @"lib\out-time.dat";

            var commitmentPeriod =
#if true
                "5days";
#else
                "50years";
#endif

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
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "3months old")]
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "1years old")]
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "5years old")]
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "10years old")]
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "15years old")]
        [InlineData(@"inp/EIR/Sr-90/Sr-90_ing.inp", "adult")]
        public void Test_EIR(string inputPath, string exposureAge)
        {
            var target = Path.GetFileNameWithoutExtension(inputPath);
            var nuclide = target.Split('_')[0];

            var expectDir = Path.Combine(TestDir, "Expect_EIR", nuclide, exposureAge.Replace(' ', '_'));
            var resultDir = Path.Combine(TestDir, "Result_EIR~", nuclide, exposureAge.Replace(' ', '_'));
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\time.dat";
            var oTimeMeshFile = @"lib\out-time.dat";

            var commitmentPeriod =
#if true
                "5days";
#else
                exposureAge == "3months old" /**/? "25450days" : // 70years - 100days = 25550days - 100days
                exposureAge == "1years old"  /**/? "69years" :   // 70years -  1years
                exposureAge == "5years old"  /**/? "65years" :   // 70years -  5years
                exposureAge == "10years old" /**/? "60years" :   // 70years - 10years
                exposureAge == "15years old" /**/? "55years" :   // 70years - 15years
                exposureAge == "adult"       /**/? "50years" :   // 75years - 25years
                throw new NotSupportedException();
#endif

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
