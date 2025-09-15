namespace FlexID.Calc.Tests;

[TestClass]
public class TestCalc
{
    [TestMethod]
    public void TestPrintCompartmentsTransfers()
    {
        var target = "test_ing.inp";

        var targetDir = TestFiles.Combine("TestCalc");
        var targetName = Path.GetFileNameWithoutExtension(target);

        // インプットファイルを読み込む。
        var data = new InputDataReader_OIR(Path.Combine(targetDir, target)).Read();

        // 詳細ログ出力の有無を設定する。
        data.PrintCompartments = true;
        data.PrintTransfers = true;

        IEnumerable<string> PrintCompartments()
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            MainRoutine_OIR.WriteOutCompartments(data, writer);

            stream.Seek(0, SeekOrigin.Begin);
            return ReadLines(stream).ToArray();
        }

        PrintCompartments().ShouldBe(
        [
            "",
            "Compartments:",
            "  inp Z Cs-134/input",
            "  acc   Cs-134/Oralcavity     O-cavity",
            "  acc   Cs-134/Oesophagus-F   Oesophag-f",
            "  acc   Cs-134/Oesophagus-S   Oesophag-s",
            "  acc   Cs-134/Stomach-con    St-cont",
            "  acc   Cs-134/SI-con         SI-cont",
            "  acc   Cs-134/SI-wall        SI-wall",
            "  exc   Cs-134/Faeces",
        ]);

        IEnumerable<string> PrintTransfers()
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            MainRoutine_OIR.WriteOutTransfers(data, writer);

            stream.Seek(0, SeekOrigin.Begin);
            return ReadLines(stream).ToArray();
        }

        PrintTransfers().ShouldBe(
        [
            "",
            "Transfers:",
            "  Cs-134/input         -> Cs-134/Oralcavity      1 = 1 * 100.00%",
            "  Cs-134/Oralcavity    -> Cs-134/Oesophagus-F  1.8 = 2 * 0.9",
            "  Cs-134/Oralcavity    -> Cs-134/Oesophagus-S  0.2 = 2 * 0.1",
            "  Cs-134/Oesophagus-F  -> Cs-134/Stomach-con     2 = 2 * 1",
            "  Cs-134/Oesophagus-S  -> Cs-134/Stomach-con     3 = 3 * 1",
            "  Cs-134/Stomach-con   -> Cs-134/SI-con          5 = 5 * 1",
            "  Cs-134/SI-wall       -> Cs-134/SI-con          2 = 2 * 1",
            "  Cs-134/SI-con        -> Cs-134/SI-wall         2 = 4 * 0.5",
            "  Cs-134/SI-con        -> Cs-134/Faeces          2 = 4 * 0.5",
        ]);
    }

    private IEnumerable<string> ReadLines(MemoryStream memoryStream)
    {
        using (var reader = new StreamReader(memoryStream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }

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
                File.ReadAllLines(actualFilePath).ShouldBe(File.ReadAllLines(expectFilePath));
            else
                File.Exists(actualFilePath).ShouldBeFalse();
        }
    }
}
