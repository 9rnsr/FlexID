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
        [DataRow("Pattern-I_&_case-2")]
        [DataRow("Pattern-II_&_case-2")]
        [DataRow("Pattern-III_&_case-2")]
        [DataRow("Pattern-I_&_case-3")]
        [DataRow("Pattern-II_&_case-3")]
        [DataRow("Pattern-III_&_case-3")]
        [DataRow("Pattern-I_&_case-4")]
        [DataRow("Pattern-II_&_case-4")]
        [DataRow("Pattern-III_&_case-4")]
        [DataRow("Pattern-I_&_case-5")]
        [DataRow("Pattern-II_&_case-5")]
        [DataRow("Pattern-III_&_case-5")]
        [DataRow("Pattern-I_&_case-6")]
        [DataRow("Pattern-II_&_case-6")]
        [DataRow("Pattern-III_&_case-6")]
        [DataRow("Pattern-I_&_case-7")]
        [DataRow("Pattern-II_&_case-7")]
        [DataRow("Pattern-III_&_case-7")]
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

            var data = new InputDataReader_OIR(inputPath).Read();

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;

            main.Main(data);

            File.Delete(Path.Combine(resultDir, target + ".log"));

            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out")),
                File.ReadAllLines(Path.Combine(resultDir, target + "_Retention.out")));
        }
    }
}
