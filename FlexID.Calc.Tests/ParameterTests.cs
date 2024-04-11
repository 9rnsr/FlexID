using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class ParameterTests
    {
        private const string ParamDir = @"TestFiles\parameter";

        [Theory]
        [MemberData(nameof(ValidCases))]
        public void TestValids(string paramFile, Program.CommandLine expect)
        {
            var paramFilePath = Path.Combine(ParamDir, paramFile);
            var lines = File.ReadAllLines(paramFilePath);
            var param = Program.GetParam(lines);
            Assert.Equal(expect, param);
        }

        public static IEnumerable<object[]> ValidCases()
        {
            // パラメータを正常に読み取る
            yield return new object[]
            {
                "param.txt",
                new Program.CommandLine
                {
                    Output = @"TestFiles\out~\test_H-3_inh-TypeM.out",
                    Input = @"TestFiles\inp\H-3\H-3_inh-TypeM.inp",
                    CalcTimeMesh = @"TestFiles\lib\time.dat",
                    OutTimeMesh = @"TestFiles\lib\out-time.dat",
                    CommitmentPeriod = "5years"
                },
            };

            // パラメータの順番がバラバラでも正常に読み取る
            yield return new object[]
            {
                "param-order.txt",
                new Program.CommandLine
                {
                    Output = @"TestFiles\out~\test_C-14_ing.out",
                    Input = @"TestFiles\inp\C-14\C-14_ing.inp",
                    CalcTimeMesh = @"TestFiles\lib\time-50y.dat",
                    OutTimeMesh = @"TestFiles\lib\out-time-50y.dat",
                    CommitmentPeriod = "15days"
                },
            };

            // コメント行を飛ばし、正常に読み取る
            yield return new object[]
            {
                "param-comment.txt",
                new Program.CommandLine
                {
                    Output = @"TestFiles\out~\test_Sr-90_ing-Other_P.out",
                    Input = @"TestFiles\inp\Sr-90\Sr-90_ing-Other_P.inp",
                    CalcTimeMesh = @"TestFiles\lib\time-10d.dat",
                    OutTimeMesh = @"TestFiles\lib\out-time-10d.dat",
                    CommitmentPeriod = "50days"
                },
            };
        }

        [Theory]
        [MemberData(nameof(ErrorCases))]
        public void TestErrors(string paramFile, string expect)
        {
            var paramFilePath = Path.Combine(ParamDir, paramFile);
            var lines = File.ReadAllLines(paramFilePath);
            var e = Assert.ThrowsAny<Exception>(() => Program.GetParam(lines));
            Assert.Equal(expect, e.Message);
        }

        public static IEnumerable<object[]> ErrorCases()
        {
            // パラメータが2つある
            yield return new[]
            {
                "paramError-Output-double.txt",
                "Specify one Output parameter.",
            };
            yield return new[]
            {
                "paramError-Input-double.txt",
                "Specify one Input parameter.",
            };
            yield return new[]
            {
                "paramError-CalcTime-double.txt",
                "Specify one CalcTimeMesh parameter.",
            };
            yield return new[]
            {
                "paramError-OutTime-double.txt",
                "Specify one OutTimeMesh parameter.",
            };
            yield return new[]
            {
                "paramError-Commit-double.txt",
                "Specify one CommitmentPeriod parameter.",
            };

            // パラメータが抜けている
            yield return new[]
            {
                "paramError-Output-missing.txt",
                "Please enter the Output parameter.",
            };
            yield return new[]
            {
                "paramError-Input-missing.txt",
                "Please enter the Input parameter.",
            };
            yield return new[]
            {
                "paramError-CalcTime-missing.txt",
                "Please enter the CalcTimeMesh parameter.",
            };
            yield return new[]
            {
                "paramError-OutTime-missing.txt",
                "Please enter the OutTimeMesh parameter.",
            };
            yield return new[]
            {
                "paramError-Commit-missing.txt",
                "Please enter the CommitmentPeriod parameter.",
            };

            yield return new[]
            {
                "paramError-Invalid.txt",
                "There is invalid parameter on line 4.",
            };
        }
    }
}
