using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class TestCalc
    {
        // 試計算
        [TestMethod]
        [DataRow("test_ing.inp")]
        public void Test(string target)
        {
            var TargetDir = TestFiles.Combine("TestCalc");
            var ExpectDir = Path.Combine(TargetDir, "Expect");
            var ResultDir = Path.Combine(TargetDir, "Result~");
            var TargetName = Path.GetFileNameWithoutExtension(target);

            Directory.CreateDirectory(ResultDir);

            var data = new InputDataReader(Path.Combine(TargetDir, target), calcProgeny: false).Read_OIR();

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= Path.Combine(ResultDir, TargetName);
            main.CalcTimeMeshPath /**/= Path.Combine(TargetDir, "time-per-1d.dat");
            main.OutTimeMeshPath  /**/= Path.Combine(TargetDir, "time-per-1d.dat");
            main.CommitmentPeriod /**/= @"5days";

            main.Main(data);

            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(ExpectDir, TargetName + "_Dose.out")),
                File.ReadAllLines(Path.Combine(ResultDir, TargetName + "_Dose.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(ExpectDir, TargetName + "_DoseRate.out")),
                File.ReadAllLines(Path.Combine(ResultDir, TargetName + "_DoseRate.out")));

            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(ExpectDir, TargetName + "_Retention.out")),
                File.ReadAllLines(Path.Combine(ResultDir, TargetName + "_Retention.out")));
            CollectionAssert.AreEqual(
                File.ReadAllLines(Path.Combine(ExpectDir, TargetName + "_Cumulative.out")),
                File.ReadAllLines(Path.Combine(ResultDir, TargetName + "_Cumulative.out")));
        }
    }
}
