using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class Analysis
    {
        private string TestDir => TestFiles.Combine("Analysis");

        [TestMethod]
        [DataRow("Pattern-I_&_case-1")]
        [DataRow("Pattern-II_&_case-1")]
        [DataRow("Pattern-III_&_case-1")]
        public void Test_OIR(string target)
        {
            var inputPath = Path.Combine(TestDir, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect");
            var resultDir = Path.Combine(TestDir, "Result~");
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = Path.Combine(TestDir, "ctime.dat");
            var oTimeMeshFile = Path.Combine(TestDir, "otime.dat");

            var commitmentPeriod = "70years";

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
    }
}
