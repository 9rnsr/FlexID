using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.ThrowsException<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$var"));

            Assert.IsTrue(evaluator.TryReadVarDecl(LineNum, "$var = 123"));
            Assert.AreEqual((123m, false), evaluator.ReadCoefficient(LineNum, "$var"));

            Assert.ThrowsException<ApplicationException>(() => evaluator.TryReadVarDecl(LineNum, "$var = 456.78"));
        }

        [TestMethod]
        public void DefineVariable2()
        {
            Assert.IsTrue(evaluator.TryReadVarDecl(LineNum, "$var = 12.3%"));
            Assert.AreEqual((0.123m, true), evaluator.ReadCoefficient(LineNum, "$var"));

            var actual = evaluator.ReadCoefficient(LineNum, "$(var + 45.6%)");
            Assert.AreEqual((0.123m + 0.456m, true), actual);
        }

        [TestMethod]
        public void CalcAddSub()
        {
            Assert.AreEqual((46m, false), evaluator.ReadCoefficient(LineNum, "$(12 + 34)"));
            Assert.AreEqual((0.46m, true), evaluator.ReadCoefficient(LineNum, "$(12% + 34%)"));

            Assert.AreEqual((22m, false), evaluator.ReadCoefficient(LineNum, "$(34 - 12)"));
            Assert.AreEqual((0.22m, true), evaluator.ReadCoefficient(LineNum, "$(34% - 12%)"));

            var errorAdd = $"Line {LineNum}: addition with inconsistent value units";
            Assert.ThrowsException<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12 + 34%)"), errorAdd);
            Assert.ThrowsException<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12% + 34)"), errorAdd);

            var errorSub = $"Line {LineNum}: subtraction with inconsistent value units";
            Assert.ThrowsException<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12 - 34%)"), errorSub);
            Assert.ThrowsException<ApplicationException>(() => evaluator.ReadCoefficient(LineNum, "$(12% - 34)"), errorSub);
        }

        [TestMethod]
        public void CalcMulDiv()
        {
            Assert.AreEqual((408m, false), evaluator.ReadCoefficient(LineNum, "$(12 * 34)"));
            Assert.AreEqual((0.0408m, true), evaluator.ReadCoefficient(LineNum, "$(12% * 34%)"));

            Assert.AreEqual((2.5m, false), evaluator.ReadCoefficient(LineNum, "$(30 / 12)"));
            Assert.AreEqual((2.5m, true), evaluator.ReadCoefficient(LineNum, "$(30% / 12%)"));

            Assert.AreEqual((4.08m, false), evaluator.ReadCoefficient(LineNum, "$(12 * 34%)"));
            Assert.AreEqual((4.08m, false), evaluator.ReadCoefficient(LineNum, "$(12% * 34)"));

            Assert.AreEqual((0.025m, false), evaluator.ReadCoefficient(LineNum, "$(30% / 12)"));
            Assert.AreEqual((250m, false), evaluator.ReadCoefficient(LineNum, "$(30 / 12%)"));
        }

        [TestMethod]
        public void CalcSItoBlood()
        {
            Assert.IsTrue(evaluator.TryReadVarDecl(LineNum, "$fA = 1E-4"));

            var expect = 1E-4m * 6 / (1 - 1E-4m);
            var actual = evaluator.ReadCoefficient(LineNum, "$(fA * 6 / (1 - fA))").value;
            Assert.AreEqual(expect, actual);
        }

        [TestMethod]
        public void CalcInputToHRTM()
        {
            Assert.IsTrue(evaluator.TryReadVarDecl(LineNum, "$fr = 0.01"));

            var expectToET2F = 0.002576836m;
            var actualToET2F = evaluator.ReadCoefficient(LineNum, "$(      fr  * (100% - 0.2%) * 25.82% )").value;
            Assert.AreEqual(expectToET2F, 0.01m * (1.0m - 0.002m) * 0.2582m);
            Assert.AreEqual(expectToET2F, actualToET2F);

            var expectToET2S = 0.255106764m;
            var actualToET2S = evaluator.ReadCoefficient(LineNum, "$( (1 - fr) * (100% - 0.2%) * 25.82% )").value;
            Assert.AreEqual(expectToET2S, (1m - 0.01m) * (1.0m - 0.002m) * 0.2582m);
            Assert.AreEqual(expectToET2S, actualToET2S);
        }
    }
}
