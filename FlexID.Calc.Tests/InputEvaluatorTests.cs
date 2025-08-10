namespace FlexID.Calc.Tests
{
    [TestClass]
    public class InputEvaluatorTests
    {
        const int LineNum = 0;
        readonly InputErrors errors;
        readonly InputEvaluator evaluator;

        public InputEvaluatorTests()
        {
            errors = new InputErrors();
            evaluator = new InputEvaluator(errors);
        }

        private (decimal value, bool isRate) SuccessReadCoefficient(int lineNum, string input)
        {
            if (evaluator.TryReadCoefficient(lineNum, input, out var result))
                return result;
            Assert.Fail();
            return default; // unreachable
        }

        private IReadOnlyList<string> FailureReadCoefficient(int lineNum, string input)
        {
            if (!evaluator.TryReadCoefficient(lineNum, input, out var _))
            {
                var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
                errors.Clear();
                return lines;
            }
            Assert.Fail();
            return default; // unreachable
        }

        [TestMethod]
        public void DefineVariable()
        {
            FailureReadCoefficient(LineNum, "$var").ShouldBe(new[] { $"undefined variable 'var'." });

            evaluator.TryReadVarDecl(LineNum, "$var = 123").ShouldBeTrue();
            SuccessReadCoefficient(LineNum, "$var").ShouldBe((123m, false));

            evaluator.TryReadVarDecl(LineNum, "$var = 456.78").ShouldBeTrue();
            SuccessReadCoefficient(LineNum, "$var").ShouldBe((456.78m, false));
        }

        [TestMethod]
        public void DefineVariable2()
        {
            evaluator.TryReadVarDecl(LineNum, "$var = 12.3%").ShouldBeTrue();
            SuccessReadCoefficient(LineNum, "$var").ShouldBe((0.123m, true));

            SuccessReadCoefficient(LineNum, "$(var + 45.6%)").ShouldBe((0.123m + 0.456m, true));
        }

        [TestMethod]
        public void CalcAddSub()
        {
            SuccessReadCoefficient(LineNum, "$(12  + 34 )").ShouldBe((46m, false));
            SuccessReadCoefficient(LineNum, "$(12% + 34%)").ShouldBe((0.46m, true));

            SuccessReadCoefficient(LineNum, "$(34  - 12 )").ShouldBe((22m, false));
            SuccessReadCoefficient(LineNum, "$(34% - 12%)").ShouldBe((0.22m, true));

            var errorAdd = new[] { $"addition with inconsistent value units" };
            FailureReadCoefficient(LineNum, "$(12  + 34%)").ShouldBe(errorAdd);
            FailureReadCoefficient(LineNum, "$(12% + 34 )").ShouldBe(errorAdd);

            var errorSub = new[] { $"subtraction with inconsistent value units" };
            FailureReadCoefficient(LineNum, "$(12  - 34%)").ShouldBe(errorSub);
            FailureReadCoefficient(LineNum, "$(12% - 34 )").ShouldBe(errorSub);
        }

        [TestMethod]
        public void CalcMulDiv()
        {
            SuccessReadCoefficient(LineNum, "$(12  * 34 )").ShouldBe((408m, false));
            SuccessReadCoefficient(LineNum, "$(12% * 34%)").ShouldBe((0.0408m, true));

            SuccessReadCoefficient(LineNum, "$(30  / 12 )").ShouldBe((2.5m, false));
            SuccessReadCoefficient(LineNum, "$(30% / 12%)").ShouldBe((2.5m, true));

            SuccessReadCoefficient(LineNum, "$(12  * 34%)").ShouldBe((4.08m, false));
            SuccessReadCoefficient(LineNum, "$(12% * 34 )").ShouldBe((4.08m, false));

            SuccessReadCoefficient(LineNum, "$(30% / 12 )").ShouldBe((0.025m, false));
            SuccessReadCoefficient(LineNum, "$(30  / 12%)").ShouldBe((250m, false));
        }

        [TestMethod]
        public void CalcSItoBlood()
        {
            evaluator.TryReadVarDecl(LineNum, "$fA = 1E-4").ShouldBeTrue();

            var expect = 1E-4m * 6 / (1 - 1E-4m);
            var actual = SuccessReadCoefficient(LineNum, "$(fA * 6 / (1 - fA))").value;
            actual.ShouldBe(expect);
        }

        [TestMethod]
        public void CalcInputToHRTM()
        {
            evaluator.TryReadVarDecl(LineNum, "$fr = 0.01").ShouldBeTrue();

            var expectToET2F = 0.002576836m;
            var actualToET2F = SuccessReadCoefficient(LineNum, "$(      fr  * (100% - 0.2%) * 25.82% )").value;
            expectToET2F.ShouldBe(0.01m * (1.0m - 0.002m) * 0.2582m);
            actualToET2F.ShouldBe(expectToET2F);

            var expectToET2S = 0.255106764m;
            var actualToET2S = SuccessReadCoefficient(LineNum, "$( (1 - fr) * (100% - 0.2%) * 25.82% )").value;
            expectToET2S.ShouldBe((1m - 0.01m) * (1.0m - 0.002m) * 0.2582m);
            expectToET2S.ShouldBe(actualToET2S);
        }
    }
}
