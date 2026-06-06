namespace FlexID;

[TestClass]
public class InputEvaluatorTests
{
    const int LineNum = 0;
    readonly InputErrors errors;
    readonly InputEvaluator evaluator;

    public InputEvaluatorTests()
    {
        errors = new InputErrors();
        evaluator = new InputEvaluator("test", errors);
    }

    private IReadOnlyList<string> TestReadVarDecl(int lineNum, string varName, string varValue)
    {
        if (evaluator.TryReadVarDecl(lineNum, varName, varValue))
            return [];

        var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
        errors.Clear();
        return lines;
    }

    private IReadOnlyList<string> TestReadCoefficient(int lineNum, string input, out (decimal value, bool isFrac) result)
    {
        if (evaluator.TryReadCoefficient(lineNum, input, out result))
            return [];

        var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
        errors.Clear();
        return lines;
    }

    [TestMethod]
    public void DefineVariable()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(LineNum, "$var", out _).ShouldBe(["Undefined variable 'var' for nuclide 'test'."]);

        TestReadVarDecl(LineNum, "var", "123").ShouldBeEmpty();

        TestReadCoefficient(LineNum, "$var", out result).ShouldBeEmpty();
        result.ShouldBe((123m, false));

        TestReadVarDecl(LineNum, "var", "456.78").ShouldBe(["Variable 'var' is already defined for nuclide 'test'."]);
    }

    [TestMethod]
    public void DefineVariable2()
    {
        (decimal value, bool isFrac) result;

        TestReadVarDecl(LineNum, "var", "12.3%").ShouldBeEmpty();

        TestReadCoefficient(LineNum, "$var", out result).ShouldBeEmpty();
        result.ShouldBe((0.123m, true));

        TestReadCoefficient(LineNum, "$(var + 45.6%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.123m + 0.456m, true));
    }

    [TestMethod]
    public void CalcAddSub()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(LineNum, "$(12  + 34 )", out result).ShouldBeEmpty();
        result.ShouldBe((46m, false));

        TestReadCoefficient(LineNum, "$(12% + 34%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.46m, true));

        TestReadCoefficient(LineNum, "$(34  - 12 )", out result).ShouldBeEmpty();
        result.ShouldBe((22m, false));

        TestReadCoefficient(LineNum, "$(34% - 12%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.22m, true));

        var errorAdd = new[] { $"Addition with inconsistent value units" };
        TestReadCoefficient(LineNum, "$(12  + 34%)", out _).ShouldBe(errorAdd);
        TestReadCoefficient(LineNum, "$(12% + 34 )", out _).ShouldBe(errorAdd);

        var errorSub = new[] { $"Subtraction with inconsistent value units" };
        TestReadCoefficient(LineNum, "$(12  - 34%)", out _).ShouldBe(errorSub);
        TestReadCoefficient(LineNum, "$(12% - 34 )", out _).ShouldBe(errorSub);
    }

    [TestMethod]
    public void CalcMulDiv()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(LineNum, "$(12  * 34 )", out result).ShouldBeEmpty(); result.ShouldBe((408m, false));
        TestReadCoefficient(LineNum, "$(12% * 34%)", out result).ShouldBeEmpty(); result.ShouldBe((0.0408m, true));

        TestReadCoefficient(LineNum, "$(30  / 12 )", out result).ShouldBeEmpty(); result.ShouldBe((2.5m, false));
        TestReadCoefficient(LineNum, "$(30% / 12%)", out result).ShouldBeEmpty(); result.ShouldBe((2.5m, true));

        TestReadCoefficient(LineNum, "$(12  * 34%)", out result).ShouldBeEmpty(); result.ShouldBe((4.08m, false));
        TestReadCoefficient(LineNum, "$(12% * 34 )", out result).ShouldBeEmpty(); result.ShouldBe((4.08m, false));

        TestReadCoefficient(LineNum, "$(30% / 12 )", out result).ShouldBeEmpty(); result.ShouldBe((0.025m, false));
        TestReadCoefficient(LineNum, "$(30  / 12%)", out result).ShouldBeEmpty(); result.ShouldBe((250m, false));
    }

    [TestMethod]
    public void CalcSItoBlood()
    {
        TestReadVarDecl(LineNum, "fA", "1E-4").ShouldBeEmpty();

        var expect = 1E-4m * 6 / (1 - 1E-4m);
        TestReadCoefficient(LineNum, "$(fA * 6 / (1 - fA))", out var actual).ShouldBeEmpty();
        actual.value.ShouldBe(expect);
    }

    [TestMethod]
    public void CalcInputToHRTM()
    {
        TestReadVarDecl(LineNum, "fr", "0.01").ShouldBeEmpty();

        (decimal value, bool isFrac) result;

        TestReadCoefficient(LineNum, "$(      fr  * (100% - 0.2%) * 25.82% )", out result);
        var expectToET2F = 0.002576836m;
        var actualToET2F = result.value;
        expectToET2F.ShouldBe(0.01m * (1.0m - 0.002m) * 0.2582m);
        actualToET2F.ShouldBe(expectToET2F);

        TestReadCoefficient(LineNum, "$( (1 - fr) * (100% - 0.2%) * 25.82% )", out result);
        var expectToET2S = 0.255106764m;
        var actualToET2S = result.value;
        expectToET2S.ShouldBe((1m - 0.01m) * (1.0m - 0.002m) * 0.2582m);
        expectToET2S.ShouldBe(actualToET2S);
    }
}
