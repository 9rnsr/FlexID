using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class TestCalc
    {
        // 試計算
        [Theory]
        [InlineData(@"test_ing.inp")]
        public void Test(string target)
        {
            var TargetDir = @"TestFiles\TestCalc";
            var ExpectDir = Path.Combine(TargetDir, "Expect");
            var ResultDir = Path.Combine(TargetDir, "Result~");
            var TargetName = Path.GetFileNameWithoutExtension(target);

            Directory.CreateDirectory(ResultDir);

            var main = new MainRoutine_OIR();
            main.InputPath = Path.Combine(TargetDir, target);
            main.OutputPath = Path.Combine(ResultDir, TargetName);
            main.CalcTimeMeshPath = Path.Combine(TargetDir, "time-per-1d.dat");
            main.OutTimeMeshPath = Path.Combine(TargetDir, "time-per-1d.dat");
            main.CommitmentPeriod = @"5days";
            main.CalcProgeny = false;

            main.Main();

            Assert.Equal(
                File.ReadLines(Path.Combine(ExpectDir, TargetName + "_Cumulative.out")),
                File.ReadLines(Path.Combine(ResultDir, TargetName + "_Cumulative.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(ExpectDir, TargetName + "_Dose.out")),
                File.ReadLines(Path.Combine(ResultDir, TargetName + "_Dose.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(ExpectDir, TargetName + "_DoseRate.out")),
                File.ReadLines(Path.Combine(ResultDir, TargetName + "_DoseRate.out")));
            Assert.Equal(
                File.ReadLines(Path.Combine(ExpectDir, TargetName + "_Retention.out")),
                File.ReadLines(Path.Combine(ResultDir, TargetName + "_Retention.out")));
        }
    }

    public class TestCalc_Sr90_ing
    {
        [Theory(Skip = "時間がかかるので普通はテストスキップする")]
        [InlineData("Sr-90_ing-Other.inp")]
        public void Test(string target)
        {
            var TargetDir = @"TestFiles\TestCalc_Sr-90_ing";
            var ExpectDir = Path.Combine(TargetDir, "Expect");
            var ResultDir = Path.Combine(TargetDir, "Result~");
            var TargetName = Path.GetFileNameWithoutExtension(target);

            Directory.CreateDirectory(ResultDir);

            var main = new MainRoutine_OIR();
            main.InputPath = Path.Combine(@"inp\OIR\Sr-90", target);
            main.OutputPath = Path.Combine(ResultDir, TargetName);
            main.CalcTimeMeshPath = @"lib\time.dat";
            main.OutTimeMeshPath = @"lib\out-time.dat";
            main.CommitmentPeriod = "50years";
            main.CalcProgeny = true;

            main.Main();
        }
    }
}
