using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;

namespace FlexID.Calc.Tests
{
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
    public class ParserTests
    {
        static InputParser<string> parser = new InputParser<string>(new StringifyVisitor());

        string Success(string input) => parser.Expr.End().Parse(input);

        void Failure(string input) => Assert.ThrowsException<ParseException>(() => parser.Expr.End().Parse(input));

        [TestMethod]
        public void TestExpr()
        {
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
        public void TestBinaryExpr()
        {
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
        public void TestUnaryExpr()
        {
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
        public void TestIdentifier()
        {
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
        public void TestNumber()
        {
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
    }
}
