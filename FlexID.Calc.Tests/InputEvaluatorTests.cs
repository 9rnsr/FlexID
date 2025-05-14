using System;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class InputEvaluatorTests
    {
        const int LineNum = 0;
        readonly InputEvaluator evaluator = new InputEvaluator();

        [Fact]
        public void DefineVariable()
        {
            Assert.Throws<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$var"));

            Assert.True(evaluator.TryReadVarDecl(LineNum, "$var = 123"));
            Assert.Equal((123m, false), evaluator.ReadCoefficient(LineNum, "$var"));

            Assert.True(evaluator.TryReadVarDecl(LineNum, "$var = 456.78"));
            Assert.Equal((456.78m, false), evaluator.ReadCoefficient(LineNum, "$var"));
        }

        [Fact]
        public void DefineVariable2()
        {
            Assert.True(evaluator.TryReadVarDecl(LineNum, "$var = 12.3%"));
            Assert.Equal((0.123m, true), evaluator.ReadCoefficient(LineNum, "$var"));

            var actual = evaluator.ReadCoefficient(LineNum, "$(var + 45.6%)");
            Assert.Equal((0.123m + 0.456m, true), actual);
        }

        [Fact]
        public void CalcAddSub()
        {
            Assert.Equal((46m, false), evaluator.ReadCoefficient(LineNum, "$(12 + 34)"));
            Assert.Equal((0.46m, true), evaluator.ReadCoefficient(LineNum, "$(12% + 34%)"));

            Assert.Equal((22m, false), evaluator.ReadCoefficient(LineNum, "$(34 - 12)"));
            Assert.Equal((0.22m, true), evaluator.ReadCoefficient(LineNum, "$(34% - 12%)"));

            ApplicationException e;

            var errorAdd = $"Line {LineNum}: addition with inconsistent value units";
            e = Assert.Throws<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12 + 34%)")); Assert.Equal(errorAdd, e.Message);
            e = Assert.Throws<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12% + 34)")); Assert.Equal(errorAdd, e.Message);

            var errorSub = $"Line {LineNum}: subtraction with inconsistent value units";
            e = Assert.Throws<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12 - 34%)")); Assert.Equal(errorSub, e.Message);
            e = Assert.Throws<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12% - 34)")); Assert.Equal(errorSub, e.Message);
        }

        [Fact]
        public void CalcMulDiv()
        {
            Assert.Equal((408m, false), evaluator.ReadCoefficient(LineNum, "$(12 * 34)"));
            Assert.Equal((0.0408m, true), evaluator.ReadCoefficient(LineNum, "$(12% * 34%)"));

            Assert.Equal((2.5m, false), evaluator.ReadCoefficient(LineNum, "$(30 / 12)"));
            Assert.Equal((2.5m, true), evaluator.ReadCoefficient(LineNum, "$(30% / 12%)"));

            Assert.Equal((4.08m, false), evaluator.ReadCoefficient(LineNum, "$(12 * 34%)"));
            Assert.Equal((4.08m, false), evaluator.ReadCoefficient(LineNum, "$(12% * 34)"));

            Assert.Equal((0.025m, false), evaluator.ReadCoefficient(LineNum, "$(30% / 12)"));
            Assert.Equal((250m, false), evaluator.ReadCoefficient(LineNum, "$(30 / 12%)"));
        }

        [Fact]
        public void CalcSItoBlood()
        {
            Assert.True(evaluator.TryReadVarDecl(LineNum, "$fA = 1E-4"));

            var expect = 1E-4m * 6 / (1 - 1E-4m);
            var actual = evaluator.ReadCoefficient(LineNum, "$(fA * 6 / (1 - fA))").value;
            Assert.Equal(expect, actual);
        }

        [Fact]
        public void CalcInputToHRTM()
        {
            Assert.True(evaluator.TryReadVarDecl(LineNum, "$fr = 0.01"));

            var expectToET2F = 0.002576836m;
            if (expectToET2F != 0.01m * (1.0m - 0.002m) * 0.2582m)
                Assert.Fail();
            var actualToET2F = evaluator.ReadCoefficient(LineNum, "$(      fr  * (100% - 0.2%) * 25.82% )").value;
            Assert.Equal(expectToET2F, actualToET2F);

            var expectToET2S = 0.255106764m;
            if (expectToET2S != (1m - 0.01m) * (1.0m - 0.002m) * 0.2582m)
                Assert.Fail();
            var actualToET2S = evaluator.ReadCoefficient(LineNum, "$( (1 - fr) * (100% - 0.2%) * 25.82% )").value;
            Assert.Equal(expectToET2S, actualToET2S);
        }
    }
}
