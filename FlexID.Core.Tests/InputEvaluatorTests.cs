namespace FlexID;

[TestClass]
public class InputEvaluatorTests
{
    readonly InputErrors errors;
    readonly InputEvaluator evaluator;

    public InputEvaluatorTests()
    {
        errors = new InputErrors();
        evaluator = new InputEvaluator("test", errors);
    }

    private IReadOnlyList<string> TestReadVarDecl(Location loc, string varName, string varValue)
    {
        if (evaluator.TryReadVarDecl(loc, varName, varValue))
            return [];

        var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
        errors.Clear();
        return lines;
    }

    private IReadOnlyList<string> TestReadCoefficient(Location loc, string input, out (decimal value, bool isFrac) result)
    {
        if (evaluator.TryReadCoefficient(loc, input, out result))
            return [];

        var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
        errors.Clear();
        return lines;
    }

    private IReadOnlyList<string> TestReadCompartment(Location loc, string input, out string result)
    {
        result = input;
        if (evaluator.TryReadCompartment(loc, ref result))
            return [];

        var lines = new Action(() => errors.RaiseIfAny()).ShouldThrow<InputErrorsException>().ErrorLines.ToArray();
        errors.Clear();
        return lines;
    }

    [TestMethod]
    public void DefineVariable()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(default, "$var", out _).ShouldBe(["Error: Undefined variable 'var' for nuclide 'test'."]);

        TestReadVarDecl(default, "var", "123").ShouldBeEmpty();

        TestReadCoefficient(default, "$var", out result).ShouldBeEmpty();
        result.ShouldBe((123m, false));

        TestReadVarDecl(default, "var", "456.78").ShouldBe(["Error: Variable 'var' is already defined for nuclide 'test'."]);
    }

    [TestMethod]
    public void DefineVariable2()
    {
        (decimal value, bool isFrac) result;

        TestReadVarDecl(default, "var", "12.3%").ShouldBeEmpty();

        TestReadCoefficient(default, "$var", out result).ShouldBeEmpty();
        result.ShouldBe((0.123m, true));

        TestReadCoefficient(default, "$(var + 45.6%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.123m + 0.456m, true));
    }

    [TestMethod]
    public void CalcAddSub()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(default, "$(12  + 34 )", out result).ShouldBeEmpty();
        result.ShouldBe((46m, false));

        TestReadCoefficient(default, "$(12% + 34%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.46m, true));

        TestReadCoefficient(default, "$(34  - 12 )", out result).ShouldBeEmpty();
        result.ShouldBe((22m, false));

        TestReadCoefficient(default, "$(34% - 12%)", out result).ShouldBeEmpty();
        result.ShouldBe((0.22m, true));

        var errorAdd = new[] { $"Error: Addition with inconsistent value units" };
        TestReadCoefficient(default, "$(12  + 34%)", out _).ShouldBe(errorAdd);
        TestReadCoefficient(default, "$(12% + 34 )", out _).ShouldBe(errorAdd);

        var errorSub = new[] { $"Error: Subtraction with inconsistent value units" };
        TestReadCoefficient(default, "$(12  - 34%)", out _).ShouldBe(errorSub);
        TestReadCoefficient(default, "$(12% - 34 )", out _).ShouldBe(errorSub);
    }

    [TestMethod]
    public void CalcMulDiv()
    {
        (decimal value, bool isFrac) result;

        TestReadCoefficient(default, "$(12  * 34 )", out result).ShouldBeEmpty(); result.ShouldBe((408m, false));
        TestReadCoefficient(default, "$(12% * 34%)", out result).ShouldBeEmpty(); result.ShouldBe((0.0408m, true));

        TestReadCoefficient(default, "$(30  / 12 )", out result).ShouldBeEmpty(); result.ShouldBe((2.5m, false));
        TestReadCoefficient(default, "$(30% / 12%)", out result).ShouldBeEmpty(); result.ShouldBe((2.5m, true));

        TestReadCoefficient(default, "$(12  * 34%)", out result).ShouldBeEmpty(); result.ShouldBe((4.08m, false));
        TestReadCoefficient(default, "$(12% * 34 )", out result).ShouldBeEmpty(); result.ShouldBe((4.08m, false));

        TestReadCoefficient(default, "$(30% / 12 )", out result).ShouldBeEmpty(); result.ShouldBe((0.025m, false));
        TestReadCoefficient(default, "$(30  / 12%)", out result).ShouldBeEmpty(); result.ShouldBe((250m, false));
    }

    [TestMethod]
    public void CalcSItoBlood()
    {
        TestReadVarDecl(default, "fA", "1E-4").ShouldBeEmpty();

        var expect = 1E-4m * 6 / (1 - 1E-4m);
        TestReadCoefficient(default, "$(fA * 6 / (1 - fA))", out var actual).ShouldBeEmpty();
        actual.value.ShouldBe(expect);
    }

    [TestMethod]
    public void CalcInputToHRTM()
    {
        TestReadVarDecl(default, "fr", "0.01").ShouldBeEmpty();

        (decimal value, bool isFrac) result;

        TestReadCoefficient(default, "$(      fr  * (100% - 0.2%) * 25.82% )", out result);
        var expectToET2F = 0.002576836m;
        var actualToET2F = result.value;
        expectToET2F.ShouldBe(0.01m * (1.0m - 0.002m) * 0.2582m);
        actualToET2F.ShouldBe(expectToET2F);

        TestReadCoefficient(default, "$( (1 - fr) * (100% - 0.2%) * 25.82% )", out result);
        var expectToET2S = 0.255106764m;
        var actualToET2S = result.value;
        expectToET2S.ShouldBe((1m - 0.01m) * (1.0m - 0.002m) * 0.2582m);
        expectToET2S.ShouldBe(actualToET2S);
    }

    [TestMethod]
    public void CalcCompartment()
    {
        string result;

        TestReadCompartment(default, "abc", out result).ShouldBeEmpty();
        result.ShouldBe("abc");

        TestReadCompartment(default, """$('abc' + 'def')""", out result).ShouldBeEmpty();
        result.ShouldBe("abcdef");

        TestReadCompartment(default, """$('a\'bc' + "d\"ef")""", out result).ShouldBeEmpty();
        result.ShouldBe("a'bcd\"ef");

        TestReadCompartment(default, "abc def", out result).ShouldBe(new[]
        {
            "Error: Expected a compartment name, not 'abc def'.",
        });
    }
}
