using Sprache;
using System.Globalization;

namespace FlexID.Calc
{
    public class Expr
    {
    }

    public class LiteralValue
    {
        public string Expr;

        public decimal Value;
    }

    public static class Expression
    {
        private static Parser<string> IdentifierParser =
            Parse.Identifier(Parse.Letter, Parse.LetterOrDigit.Or(Parse.Char('_')));

        public LiteralValue Calc(string expr)
        {
            if (decimal.TryParse(expr, NumberStyles.Float, null, out decimal v))
            {

            }
        }
    }
}
