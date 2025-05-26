namespace FlexID.Calc.Tests;

using Sprache;

/// <summary>
/// テストのため式構造を文字列化する。
/// </summary>
class StringifyVisitor : Visitor<string>
{
    public string Var(string ident) => ident;
    public string Number(string value, string unit) => $"{value}{unit}";
    public string Pos(string expr) => $"+{expr}";
    public string Neg(string expr) => $"-{expr}";
    public string Add(string left, string right) => $"({left} + {right})";
    public string Sub(string left, string right) => $"({left} - {right})";
    public string Mul(string left, string right) => $"({left} * {right})";
    public string Div(string left, string right) => $"({left} / {right})";
}

[TestClass]
public class InputParserTests
{
    InputParser<string> parser = new InputParser<string>(new StringifyVisitor());

    (Func<string, string> Success, Action<string> Failure) MakeTesters(Parser<string> parser) =>
        (input => parser.End().Parse(input), input => Assert.ThrowsException<ParseException>(() => parser.End().Parse(input)));

    [TestMethod]
    public void ParseVar()
    {
        var (Success, Failure) = MakeTesters(parser.VarExpr);

        Success("name");
        Success("fn_1");

        Failure("_abc");
        Failure("'abc");
        Failure("1abc");
        Failure("abc'");
        Failure("abc 0");
        Failure("abc.def");
    }

    [TestMethod]
    public void ParseNumber()
    {
        var (Success, Failure) = MakeTesters(parser.NumberExpr);

        Success("123");
        Success("123.456");
        Success("0123");
        Success(".123");

        Failure("123.");

        Success("123E01");      /**/ Success("123e01");
        Success("123E+01");     /**/ Success("123e+01");
        Success("123E-01");     /**/ Success("123e-01");
        Success("123.456E01");  /**/ Success("123.456e01");
        Success("123.456E+01"); /**/ Success("123.456e+01");
        Success("123.456E-01"); /**/ Success("123.456e-01");

        Success("98.7%");
        Success("3.14E-01%");

        Failure("123xyz");
    }

    [TestMethod]
    public void ParseUnaryExpr()
    {
        var (Success, Failure) = MakeTesters(parser.Expr);

        Success("-name");

        Success("+123");
        Success("-123.456");
        Success("+123e01");
        Success("-123e01");
        Success("+123.456E+01");
        Success("-123.456E-01");

        Success("+98.7%");
        Success("-3.14%");
    }

    [TestMethod]
    public void ParseBinaryExpr()
    {
        var (Success, Failure) = MakeTesters(parser.Expr);

        Assert.AreEqual("(123 * 456)", Success("123*456"));
        Assert.AreEqual("(123 / 456)", Success(" 123 / 456 "));
        Assert.AreEqual("((12 * 34) / 56)", Success("12 * 34 / 56"));
        Assert.AreEqual("(((12 * 34) / 56) * 78)", Success("12 * 34 / 56 * 78"));

        Assert.AreEqual("(123 + 456)", Success("123+456"));
        Assert.AreEqual("(123 - 456)", Success(" 123 - 456 "));
        Assert.AreEqual("((12 + 34) - 56)", Success("12 + 34 - 56"));
        Assert.AreEqual("(12 + (34 * 56))", Success("12 + 34 * 56"));
        Assert.AreEqual("((12 * 34) + (56 / 78))", Success("12 * 34 + 56 / 78"));
    }

    [TestMethod]
    public void ParseExpr()
    {
        var (Success, Failure) = MakeTesters(parser.Expr);

        Assert.AreEqual("identifier", /**/ Success("identifier"));
        Assert.AreEqual("1234",       /**/ Success("1234"));
        Assert.AreEqual("3.1415",     /**/ Success("3.1415"));

        Assert.AreEqual("identifier", /**/ Success("(identifier)"));
        Assert.AreEqual("1234",       /**/ Success("(1234)"));
        Assert.AreEqual("3.1415",     /**/ Success("(3.1415)"));

        Success("-(1.2 + +(a * b) - (-c / +d))");

        Failure("(123");
        Failure("456)");
    }

    [TestMethod]
    public void ParseCoefficient()
    {
        var (Success, Failure) = MakeTesters(parser.Coefficient);

        Assert.AreEqual("123", Success("123"));
        Assert.AreEqual("123.45", Success("123.45"));
        Assert.AreEqual("1.23E-04", Success("1.23E-04"));
        Assert.AreEqual("+123.45", Success("+123.45"));
        Assert.AreEqual("-1.23E-04", Success("-1.23E-04"));

        Assert.AreEqual("a", Success("$a"));

        Assert.AreEqual("((a * 12%) + 1.5)", Success("$(a * 12% + 1.5)"));

        Failure("a");
        Failure("(a * 12% + 1.5)");

        Failure("$(12 + 34) / 2");
    }
}
