using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class TrialCalcTests
    {
        private string TestDir => TestFiles.Combine("TrialCalc");

        [TestMethod]
        [DataRow("Ba-133_ing-Insoluble")]
        [DataRow("Ba-133_ing-Soluble")]
        [DataRow("Ba-133_inh-TypeF")]
        [DataRow("Ba-133_inh-TypeM")]
        [DataRow("Ba-133_inh-TypeS")]
        [DataRow("C-14_ing")]
        [DataRow("C-14_inh-TypeF")]
        [DataRow("C-14_inh-TypeF-Barium")]
        [DataRow("C-14_inh-TypeF-Gas")]
        [DataRow("C-14_inh-TypeM")]
        [DataRow("C-14_inh-TypeS")]
        [DataRow("Ca-45_ing")]
        [DataRow("Ca-45_inh-TypeF")]
        [DataRow("Ca-45_inh-TypeM")]
        [DataRow("Ca-45_inh-TypeS")]
        [DataRow("Cs-134_ing-Insoluble")]
        [DataRow("Cs-134_ing-Unspecified")]
        [DataRow("Cs-134_inh-TypeF")]
        [DataRow("Cs-134_inh-TypeM")]
        [DataRow("Cs-134_inh-TypeS")]
        [DataRow("Cs-137_ing-Insoluble")]
        [DataRow("Cs-137_ing-Unspecified")]
        [DataRow("Cs-137_inh-TypeF")]
        [DataRow("Cs-137_inh-TypeM")]
        [DataRow("Cs-137_inh-TypeS")]
        [DataRow("Fe-59_ing")]
        [DataRow("Fe-59_inh-TypeF")]
        [DataRow("Fe-59_inh-TypeM")]
        [DataRow("Fe-59_inh-TypeS")]
        [DataRow("H-3_ing-Insoluble")]
        [DataRow("H-3_ing-Organic")]
        [DataRow("H-3_ing-Soluble")]
        [DataRow("H-3_inh-TypeF-Gas")]
        [DataRow("H-3_inh-TypeF-Organic")]
        [DataRow("H-3_inh-TypeF-Tritide")]
        [DataRow("H-3_inh-TypeM")]
        [DataRow("H-3_inh-TypeS")]
        [DataRow("I-129_ing")]
        [DataRow("I-129_inh-TypeF")]
        [DataRow("I-129_inh-TypeM")]
        [DataRow("I-129_inh-TypeS")]
        [DataRow("Pu-238_ing-Insoluble")]
        [DataRow("Pu-238_ing-Unidentified")]
        [DataRow("Pu-238_inh-TypeF")]
        [DataRow("Pu-238_inh-TypeM")]
        [DataRow("Pu-238_inh-TypeS")]
        [DataRow("Pu-239_ing-Insoluble")]
        [DataRow("Pu-239_ing-Unidentified")]
        [DataRow("Pu-239_inh-TypeF")]
        [DataRow("Pu-239_inh-TypeM")]
        [DataRow("Pu-239_inh-TypeS")]
        [DataRow("Pu-239_inj")]
        [DataRow("Pu-240_ing-Insoluble")]
        [DataRow("Pu-240_ing-Unidentified")]
        [DataRow("Pu-240_inh-TypeF")]
        [DataRow("Pu-240_inh-TypeM")]
        [DataRow("Pu-240_inh-TypeS")]
        [DataRow("Pu-241_ing-Insolube")]
        [DataRow("Pu-241_ing-Unidentified")]
        [DataRow("Pu-241_inh-TypeF")]
        [DataRow("Pu-241_inh-TypeM")]
        [DataRow("Pu-241_inh-TypeS")]
        [DataRow("Pu-242_ing-Insoluble")]
        [DataRow("Pu-242_ing-Unidentified")]
        [DataRow("Pu-242_inh-TypeF")]
        [DataRow("Pu-242_inh-TypeM")]
        [DataRow("Pu-242_inh-TypeS")]
        [DataRow("Ra-223_inh-TypeF")]
        [DataRow("Ra-226_ing")]
        [DataRow("Ra-226_inh-TypeF")]
        [DataRow("Ra-226_inh-TypeM")]
        [DataRow("Ra-226_inh-TypeS")]
        [DataRow("Sr-90_ing-Other")]
        [DataRow("Sr-90_ing-Titanate")]
        [DataRow("Sr-90_inh-TypeF")]
        [DataRow("Sr-90_inh-TypeM")]
        [DataRow("Sr-90_inh-TypeS")]
        [DataRow("Tc-99_ing")]
        [DataRow("Tc-99_inh-TypeF")]
        [DataRow("Tc-99_inh-TypeM")]
        [DataRow("Tc-99_inh-TypeS")]
        [DataRow("Zn-65_ing")]
        [DataRow("Zn-65_inh-TypeF")]
        [DataRow("Zn-65_inh-TypeM")]
        [DataRow("Zn-65_inh-TypeS")]
        public void Test_OIR(string target)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "OIR", nuclide, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_OIR", nuclide);
            var resultDir = Path.Combine(TestDir, "Result_OIR~", nuclide);
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\TimeMesh\time.dat";
            var oTimeMeshFile = Path.Combine(TestDir, "out-time.dat");

            var commitmentPeriod = "50years";

            var main = new MainRoutine_OIR();
            main.InputPath        /**/= inputPath;
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;
            main.CalcProgeny      /**/= true;

            main.Main();

            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Cumulative.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Cumulative.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Dose.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_DoseRate.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Retention.out")));
        }

        [TestMethod]
        [DataRow("Sr-90_ing", "3months old")]
        [DataRow("Sr-90_ing", "1years old")]
        [DataRow("Sr-90_ing", "5years old")]
        [DataRow("Sr-90_ing", "10years old")]
        [DataRow("Sr-90_ing", "15years old")]
        [DataRow("Sr-90_ing", "Adult")]
        public void Test_EIR(string target, string exposureAge)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "EIR", nuclide, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_EIR", nuclide, exposureAge.Replace(' ', '_'));
            var resultDir = Path.Combine(TestDir, "Result_EIR~", nuclide, exposureAge.Replace(' ', '_'));
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\TimeMesh\time.dat";
            var oTimeMeshFile = Path.Combine(TestDir, "out-time.dat");

            var commitmentPeriod =
                exposureAge == "3months old" /**/? "25450days" : // 70years - 100days = 25550days - 100days
                exposureAge == "1years old"  /**/? "69years" :   // 70years -  1years
                exposureAge == "5years old"  /**/? "65years" :   // 70years -  5years
                exposureAge == "10years old" /**/? "60years" :   // 70years - 10years
                exposureAge == "15years old" /**/? "55years" :   // 70years - 15years
                exposureAge == "Adult"       /**/? "50years" :   // 75years - 25years
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

            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Cumulative.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Cumulative.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Dose.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_DoseRate.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Retention.out")));
        }
    }
}
