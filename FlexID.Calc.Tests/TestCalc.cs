using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class TestCalc
    {
        /// <summary>
        /// テスト用計算ケースを使った計算処理の実行を確認する。
        /// アウトプットファイルの出力切り替え機能についても同時にテストしている。
        /// </summary>
        [TestMethod]
        [DataRow("test_ing.inp", true,  /**/true,  /**/true,  /**/true  /**/)]
        [DataRow("test_ing.inp", true,  /**/true,  /**/true,  /**/false /**/)]
        [DataRow("test_ing.inp", true,  /**/true,  /**/false, /**/true  /**/)]
        [DataRow("test_ing.inp", true,  /**/true,  /**/false, /**/false /**/)]
        [DataRow("test_ing.inp", true,  /**/false, /**/true,  /**/true  /**/)]
        [DataRow("test_ing.inp", true,  /**/false, /**/true,  /**/false /**/)]
        [DataRow("test_ing.inp", true,  /**/false, /**/false, /**/true  /**/)]
        [DataRow("test_ing.inp", true,  /**/false, /**/false, /**/false /**/)]
        [DataRow("test_ing.inp", false, /**/true,  /**/true,  /**/true  /**/)]
        [DataRow("test_ing.inp", false, /**/true,  /**/true,  /**/false /**/)]
        [DataRow("test_ing.inp", false, /**/true,  /**/false, /**/true  /**/)]
        [DataRow("test_ing.inp", false, /**/true,  /**/false, /**/false /**/)]
        [DataRow("test_ing.inp", false, /**/false, /**/true,  /**/true  /**/)]
        [DataRow("test_ing.inp", false, /**/false, /**/true,  /**/false /**/)]
        [DataRow("test_ing.inp", false, /**/false, /**/false, /**/true  /**/)]
        [DataRow("test_ing.inp", false, /**/false, /**/false, /**/false /**/)]
        public void Test(string target, bool outDose, bool outDoseRate, bool outActRete, bool outActCumu)
        {
            var flagsD = $"_dose({(outDose ? "D" : "")}{(outDoseRate ? "R" : "")})";
            var flagsA = $"_act({(outActRete ? "R" : "")}{(outActCumu ? "C" : "")})";

            var TargetDir = TestFiles.Combine("TestCalc");
            var ExpectDir = Path.Combine(TargetDir, "Expect");
            var ResultDir = Path.Combine(TargetDir, $"Result{flagsD}{flagsA}~");
            var TargetName = Path.GetFileNameWithoutExtension(target);

            Directory.CreateDirectory(ResultDir);
            foreach (var previousOutFile in Directory.EnumerateFiles(ResultDir))
                File.Delete(previousOutFile);

            var data = new InputDataReader(Path.Combine(TargetDir, target)).Read_OIR();

            data.OutputDose = outDose;
            data.OutputDoseRate = outDoseRate;
            data.OutputRetention = outActRete;
            data.OutputCumulative = outActCumu;

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= Path.Combine(ResultDir, TargetName);
            main.CalcTimeMeshPath /**/= Path.Combine(TargetDir, "time-per-1d.dat");
            main.OutTimeMeshPath  /**/= Path.Combine(TargetDir, "time-per-1d.dat");
            main.CommitmentPeriod /**/= @"5days";

            main.Main(data);

            var expectDose = Path.Combine(ExpectDir, TargetName + "_Dose.out");
            var actualDose = Path.Combine(ResultDir, TargetName + "_Dose.out");
            if (outDose)
                CollectionAssert.AreEqual(File.ReadAllLines(expectDose), File.ReadAllLines(actualDose));
            else
                Assert.IsFalse(File.Exists(actualDose));

            var expectDoseRate = Path.Combine(ExpectDir, TargetName + "_DoseRate.out");
            var actualDoseRate = Path.Combine(ResultDir, TargetName + "_DoseRate.out");
            if (outDoseRate)
                CollectionAssert.AreEqual(File.ReadAllLines(expectDoseRate), File.ReadAllLines(actualDoseRate));
            else
                Assert.IsFalse(File.Exists(actualDoseRate));

            var expectActRete = Path.Combine(ExpectDir, TargetName + "_Retention.out");
            var actualActRete = Path.Combine(ResultDir, TargetName + "_Retention.out");
            if (outActRete)
                CollectionAssert.AreEqual(File.ReadAllLines(expectActRete), File.ReadAllLines(actualActRete));
            else
                Assert.IsFalse(File.Exists(actualActRete));

            var expectActCumu = Path.Combine(ExpectDir, TargetName + "_Cumulative.out");
            var actualActCumu = Path.Combine(ResultDir, TargetName + "_Cumulative.out");
            if (outActCumu)
                CollectionAssert.AreEqual(File.ReadAllLines(expectActCumu), File.ReadAllLines(actualActCumu));
            else
                Assert.IsFalse(File.Exists(actualActCumu));
        }
    }
}
