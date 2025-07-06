using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class InputEvaluatorTests
    {
        const int LineNum = 0;
        readonly InputEvaluator evaluator = new InputEvaluator();

        [TestMethod]
        public void DefineVariable()
        {
            new Action(() => evaluator.ReadCoefficient(LineNum, "$var")).ShouldThrow<ApplicationException>();

            evaluator.TryReadVarDecl(LineNum, "$var = 123").ShouldBeTrue();
            evaluator.ReadCoefficient(LineNum, "$var").ShouldBe((123m, false));

            evaluator.TryReadVarDecl(LineNum, "$var = 456.78").ShouldBeTrue();
            evaluator.ReadCoefficient(LineNum, "$var").ShouldBe((456.78m, false));
        }

        [TestMethod]
        public void DefineVariable2()
        {
            evaluator.TryReadVarDecl(LineNum, "$var = 12.3%").ShouldBeTrue();
            evaluator.ReadCoefficient(LineNum, "$var").ShouldBe((0.123m, true));

            var actual = evaluator.ReadCoefficient(LineNum, "$(var + 45.6%)");
            actual.ShouldBe((0.123m + 0.456m, true));
        }

        [TestMethod]
        public void CalcAddSub()
        {
            evaluator.ReadCoefficient(LineNum, "$(12  + 34 )").ShouldBe((46m, false));
            evaluator.ReadCoefficient(LineNum, "$(12% + 34%)").ShouldBe((0.46m, true));

            evaluator.ReadCoefficient(LineNum, "$(34  - 12 )").ShouldBe((22m, false));
            evaluator.ReadCoefficient(LineNum, "$(34% - 12%)").ShouldBe((0.22m, true));

            var errorAdd = $"Line {LineNum}: addition with inconsistent value units";
            new Action(() => evaluator.ReadCoefficient(LineNum, "$(12  + 34%)")).ShouldThrow<ApplicationException>().Message.ShouldBe(errorAdd);
            new Action(() => evaluator.ReadCoefficient(LineNum, "$(12% + 34 )")).ShouldThrow<ApplicationException>().Message.ShouldBe(errorAdd);

            var errorSub = $"Line {LineNum}: subtraction with inconsistent value units";
            new Action(() => evaluator.ReadCoefficient(LineNum, "$(12  - 34%)")).ShouldThrow<ApplicationException>().Message.ShouldBe(errorSub);
            new Action(() => evaluator.ReadCoefficient(LineNum, "$(12% - 34 )")).ShouldThrow<ApplicationException>().Message.ShouldBe(errorSub);
        }

        [TestMethod]
        public void CalcMulDiv()
        {
            evaluator.ReadCoefficient(LineNum, "$(12  * 34 )").ShouldBe((408m, false));
            evaluator.ReadCoefficient(LineNum, "$(12% * 34%)").ShouldBe((0.0408m, true));

            evaluator.ReadCoefficient(LineNum, "$(30  / 12 )").ShouldBe((2.5m, false));
            evaluator.ReadCoefficient(LineNum, "$(30% / 12%)").ShouldBe((2.5m, true));

            evaluator.ReadCoefficient(LineNum, "$(12  * 34%)").ShouldBe((4.08m, false));
            evaluator.ReadCoefficient(LineNum, "$(12% * 34 )").ShouldBe((4.08m, false));

            evaluator.ReadCoefficient(LineNum, "$(30% / 12 )").ShouldBe((0.025m, false));
            evaluator.ReadCoefficient(LineNum, "$(30  / 12%)").ShouldBe((250m, false));
        }

        [TestMethod]
        public void CalcSItoBlood()
        {
            evaluator.TryReadVarDecl(LineNum, "$fA = 1E-4").ShouldBeTrue();

            var expect = 1E-4m * 6 / (1 - 1E-4m);
            var actual = evaluator.ReadCoefficient(LineNum, "$(fA * 6 / (1 - fA))").value;
            actual.ShouldBe(expect);
        }

        [TestMethod]
        public void CalcInputToHRTM()
        {
            evaluator.TryReadVarDecl(LineNum, "$fr = 0.01").ShouldBeTrue();

            var expectToET2F = 0.002576836m;
            var actualToET2F = evaluator.ReadCoefficient(LineNum, "$(      fr  * (100% - 0.2%) * 25.82% )").value;
            expectToET2F.ShouldBe(0.01m * (1.0m - 0.002m) * 0.2582m);
            actualToET2F.ShouldBe(expectToET2F);

            var expectToET2S = 0.255106764m;
            var actualToET2S = evaluator.ReadCoefficient(LineNum, "$( (1 - fr) * (100% - 0.2%) * 25.82% )").value;
            expectToET2S.ShouldBe((1m - 0.01m) * (1.0m - 0.002m) * 0.2582m);
            expectToET2S.ShouldBe(actualToET2S);
        }
    }
}
