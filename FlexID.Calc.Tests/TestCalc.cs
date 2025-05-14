using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public void TestOutput(string target, bool outDose, bool outDoseRate, bool outActRete, bool outActCumu)
        {
            var targetDir = TestFiles.Combine("TestCalc");
            var targetName = Path.GetFileNameWithoutExtension(target);

            var flagsD = $"_dose({(outDose ? "D" : "")}{(outDoseRate ? "R" : "")})";
            var flagsA = $"_act({(outActRete ? "R" : "")}{(outActCumu ? "C" : "")})";

            var expectDir = Path.Combine(targetDir, "Expect");
            var resultDir = Path.Combine(targetDir, $"Result~/output{flagsD}{flagsA}");

            // 結果を出力する空のフォルダを準備する。
            Directory.CreateDirectory(resultDir);
            foreach (var previousOutFile in Directory.EnumerateFiles(resultDir))
                File.Delete(previousOutFile);

            // インプットファイルを読み込む。
            var data = new InputDataReader_OIR(Path.Combine(targetDir, target)).Read();

            // アウトプットファイル書き出しの有無を設定する。
            data.OutputDose = outDose;
            data.OutputDoseRate = outDoseRate;
            data.OutputRetention = outActRete;
            data.OutputCumulative = outActCumu;

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= Path.Combine(resultDir, targetName);
            main.CalcTimeMeshPath /**/= Path.Combine(targetDir, "time-per-1d.dat");
            main.OutTimeMeshPath  /**/= Path.Combine(targetDir, "time-per-1d.dat");
            main.CommitmentPeriod /**/= @"5days";

            // 計算を実行する。
            main.Main(data);

            // アウトプットファイルの出力有無と、出力されている場合はその内容確認を行う。
            CheckOutputFile(targetName + "_Dose.out", outDose);
            CheckOutputFile(targetName + "_DoseRate.out", outDoseRate);
            CheckOutputFile(targetName + "_Retention.out", outActRete);
            CheckOutputFile(targetName + "_Cumulative.out", outActCumu);

            void CheckOutputFile(string fileName, bool outputFlag)
            {
                var expectFilePath = Path.Combine(expectDir, fileName);
                var actualFilePath = Path.Combine(resultDir, fileName);
                if (outputFlag)
                    CollectionAssert.AreEqual(File.ReadAllLines(expectFilePath), File.ReadAllLines(actualFilePath));
                else
                    Assert.IsFalse(File.Exists(actualFilePath));
            }
        }

        /// <summary>
        /// OIR互換排泄コンパートメントの設定処理を確認する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="nuclide"></param>
        /// <param name="excretas"></param>
        [TestMethod]
        [DataRow("test_excreta.inp", "")]
        [DataRow("test_excreta.inp", "", "Faeces")]
        [DataRow("test_excreta.inp", "", "Urine")]
        [DataRow("test_excreta.inp", "", "Urine", "Faeces")]
        [DataRow("test_excreta.inp", "Mo-93", "")]
        [DataRow("test_excreta.inp", "Mo-93", "Faeces")]
        [DataRow("test_excreta.inp", "Mo-93", "Urine")]
        [DataRow("test_excreta.inp", "Mo-93", "Urine", "Faeces")]
        [DataRow("test_excreta.inp", "Nb-93m", "")]
        [DataRow("test_excreta.inp", "Nb-93m", "Faeces")]
        [DataRow("test_excreta.inp", "Nb-93m", "Urine")]
        [DataRow("test_excreta.inp", "Nb-93m", "Urine", "Faeces")]
        public void TestExcreta(string target, string nuclide, params string[] excretas)
        {
            var targetDir = TestFiles.Combine("TestCalc");
            var targetName = Path.GetFileNameWithoutExtension(target);

            var flags = $"({string.Join(",", excretas)})";
            if (!string.IsNullOrWhiteSpace(nuclide)) flags = $"_{nuclide}" + flags;

            var expectDir = Path.Combine(targetDir, "Expect");
            var resultDir = Path.Combine(targetDir, $"Result~/excreta{flags}");

            // 結果を出力する空のフォルダを準備する。
            Directory.CreateDirectory(resultDir);
            foreach (var previousOutFile in Directory.EnumerateFiles(resultDir))
                File.Delete(previousOutFile);

            var input = InputWithParameter(Path.Combine(targetDir, target), nuclide, new[]
            {
                "ExcretaCompartments = " + string.Join(" ", excretas),
            });

            // インプットファイルを読み込む。
            var data = new InputDataReader_OIR(input).Read();

            data.OutputDose = false;
            data.OutputDoseRate = false;
            data.OutputRetention = true;
            data.OutputCumulative = false;

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= Path.Combine(resultDir, targetName);
            main.CalcTimeMeshPath /**/= Path.Combine(targetDir, "time-lessthan-1day.dat");
            main.OutTimeMeshPath  /**/= Path.Combine(targetDir, "time-lessthan-1day.dat");
            main.CommitmentPeriod /**/= @"15days";

            // 計算を実行する。
            main.Main(data);

            var expectFilePath = Path.Combine(expectDir, $"{targetName}{flags}.out");
            var actualFilePath = Path.Combine(resultDir, $"{targetName}_Retention.out");
            CollectionAssert.AreEqual(File.ReadAllLines(expectFilePath), File.ReadAllLines(actualFilePath));
        }

        StreamReader InputWithParameter(string path, string nuclide, IEnumerable<string> parameters)
        {
            var mstream = new MemoryStream();

            using (var writer = new StreamWriter(mstream, Encoding.UTF8, 1024, leaveOpen: true))
            using (var reader = new StreamReader(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line is null)
                        break;
                    if (line == "[nuclide]")
                    {
                        if (string.IsNullOrWhiteSpace(nuclide))
                            writer.WriteLine("[parameter]");
                        else
                            writer.WriteLine($"[{nuclide}:parameter]");
                        foreach (var line2 in parameters)
                        {
                            writer.WriteLine(line2);
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine(line);
                }
            }

            mstream.Seek(0, SeekOrigin.Begin);

            return new StreamReader(mstream);
        }
    }
}
