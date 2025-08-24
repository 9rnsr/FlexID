using System.Globalization;
using Sprache;
using static Sprache.Parse;

namespace FlexID.Calc;

/// <summary>
/// インプットにおける文法とその解析処理を提供する。
/// </summary>
/// <typeparam name="T">解析結果を表現する型。</typeparam>
public class InputParser<T>
{
    public Parser<string> Identifier { get; }

    public Parser<T> VarExpr { get; }

    public Parser<T> NumberExpr { get; }

    public Parser<T> Expr { get; }

    public Parser<(string ident, T expr)> VarDecl { get; }

    public Parser<T> Coefficient { get; }

    public InputParser(Visitor<T> visitor)
    {
        Parser<string> OrEmpty<X>(Parser<X> parser) =>
            parser.Optional().Select(x => x.IsDefined ? x.Get().ToString() : "");

        Identifier =
            from first in Letter
            from remain in LetterOrDigit.Or(Char('_')).Many().Text()
            select $"{first}{remain}";

        VarExpr =
            from id in Identifier
            select visitor.Var(id);

        var ExponentPart =
            from e in Chars("Ee")
            from sign in OrEmpty(Chars("+-"))
            from num in Number
            select $"{e}{sign}{num}";

        NumberExpr =
            from num in DecimalInvariant
            from exponent in OrEmpty(ExponentPart)
            from unit in OrEmpty(Char('%'))
            select visitor.Number($"{num}{exponent}", unit);

        var Parenthesis = Ref(() => Expr).Contained(Char('('), Char(')'));

        var Primary = VarExpr.Or(NumberExpr).Or(Parenthesis);

        var Prefix =
            from sign in OrEmpty(Chars("+-"))
            from expr in Primary
            select (sign == "+" ? visitor.Pos(expr) :
                    sign == "-" ? visitor.Neg(expr) : expr);

        var MulDiv = ChainOperator(Chars("*/"), Prefix.Token(), (op, left, right) =>
            op == '*' ? visitor.Mul(left, right) :
                        visitor.Div(left, right));

        var AddSub = ChainOperator(Chars("+-"), MulDiv.Token(), (op, left, right) =>
            op == '+' ? visitor.Add(left, right) :
                        visitor.Sub(left, right));

        Expr = AddSub;

        VarDecl =
            from _marker in Char('$')
            from ident in Identifier.Text()
            from _assign in String("=").Token()
            from initializer in Expr
            select (ident, initializer);

        var SignedNumerExpr =
            from sign in OrEmpty(Chars("+-"))
            from expr in NumberExpr
            select (sign == "+" ? visitor.Pos(expr) :
                    sign == "-" ? visitor.Neg(expr) : expr);

        Coefficient =
            from expr in SignedNumerExpr.Or(Char('$').Then(_ => VarExpr.Or(Parenthesis)))
            select expr;
    }
}

/// <summary>
/// 式ツリーを解釈するクラスが実装すべきインターフェース。
/// </summary>
/// <typeparam name="T"></typeparam>
public interface Visitor<T>
{
    T Var(string ident);
    T Number(string value, string unit);
    T Pos(T expr);
    T Neg(T expr);
    T Add(T left, T right);
    T Sub(T left, T right);
    T Mul(T left, T right);
    T Div(T left, T right);
}

#if false
public abstract class Expr { }
class VarExpr : Expr { public VarExpr(string id) { Ident = id; } public string Ident; }
class NumberExpr : Expr { public NumberExpr(string v) { Value = v; } public string Value; }
class PosExpr : Expr { public PosExpr(Expr e) { Oper = e; } Expr Oper { get; } }
class NegExpr : Expr { public NegExpr(Expr e) { Oper = e; } Expr Oper { get; } }
class AddExpr : Expr { public AddExpr(Expr l, Expr r) { Left = l; Right = r; } Expr Left { get; } Expr Right { get; } }
class SubExpr : Expr { public SubExpr(Expr l, Expr r) { Left = l; Right = r; } Expr Left { get; } Expr Right { get; } }
class MulExpr : Expr { public MulExpr(Expr l, Expr r) { Left = l; Right = r; } Expr Left { get; } Expr Right { get; } }
class DivExpr : Expr { public DivExpr(Expr l, Expr r) { Left = l; Right = r; } Expr Left { get; } Expr Right { get; } }

public class ExpressionVisitor : Visitor<Expr>
{
    public Expr Var(string ident) => new VarExpr(ident);
    public Expr Number(string value, string unit) => new NumberExpr(value);
    public Expr Pos(Expr expr) => new PosExpr(expr);
    public Expr Neg(Expr expr) => new NegExpr(expr);
    public Expr Add(Expr left, Expr right) => new AddExpr(left, right);
    public Expr Sub(Expr left, Expr right) => new SubExpr(left, right);
    public Expr Mul(Expr left, Expr right) => new MulExpr(left, right);
    public Expr Div(Expr left, Expr right) => new DivExpr(left, right);
}
#endif

/// <summary>
/// インプット上の変数定義や部分式の評価を行う。
/// </summary>
public class InputEvaluator : Visitor<(decimal v, bool r)>
{
    private readonly InputErrors errors;

    private readonly InputParser<(decimal v, bool r)> parser;

    private readonly Dictionary<string, (decimal, bool)> variables = [];

    private int lineNum;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputEvaluator(InputErrors errors)
    {
        this.errors = errors;
        this.parser = new InputParser<(decimal, bool)>(this);
    }

    /// <summary>
    /// 変数の定義行の場合にこれの解釈を行う。
    /// </summary>
    /// <param name="lineNum">インプットの行番号。</param>
    /// <param name="input">入力文字列。</param>
    /// <returns>変数の定義行の場合はその定義値をテーブルに追加し<see langword="true"/>を返す。</returns>
    public bool TryReadVarDecl(int lineNum, string input)
    {
        this.lineNum = lineNum;

        if (input is null)
            return false;

        var result = parser.VarDecl.Token().End().TryParse(input);
        if (!result.WasSuccessful)
            return false;

        var (ident, initializer) = result.Value;

        // 変数定義の上書きを許可する。
        // if (variables.TryGetValue(ident, out _))
        //     throw Program.Error($"Line {lineNum}: Variable '{ident}' is already defined.");

        variables[ident] = initializer;

        return true;
    }

    public bool TryReadCoefficient(int lineNum, string input, out (decimal value, bool isFrac) result)
    {
        this.lineNum = lineNum;
        result = default;

        IResult<(decimal v, bool r)> r;
        try
        {
            r = parser.Coefficient.Token().End().TryParse(input);
        }
        catch (InputErrorsException ex)
        {
            errors.AddErrors(ex);
            return false;
        }
        catch (ArithmeticException ex)
        {
            errors.AddError(lineNum, $"Transfer coefficient evaluation failed: {ex.Message}.");
            return false;
        }
        if (!r.WasSuccessful)
        {
            errors.AddError(lineNum, $"Transfer coefficient should be evaluated to a number, not '{input}'.");
            return false;
        }

        var (expr, isFrac) = r.Value;
        //Debug.WriteLine($"Line {lineNum} '{input}' ==> {expr * (isFrac ? 100 : 1)}{(isFrac ? "%" : "")}");
        result = r.Value;
        return true;
    }

    public (decimal v, bool r) Var(string ident)
    {
        // 定義されていない変数の使用に対してエラーを報告する。
        if (!variables.TryGetValue(ident, out var v))
            throw new InputErrorsException(lineNum, $"undefined variable '{ident}'.");
        return v;
    }

    public (decimal v, bool r) Number(string input, string unit)
    {
        var value = decimal.Parse(input, NumberStyles.Float);
        var isFrac = (unit == "%");
        if (isFrac)
            value = value / 100;
        return (value, isFrac);
    }

    public (decimal v, bool r) Pos((decimal v, bool r) oper) => (+oper.v, oper.r);

    public (decimal v, bool r) Neg((decimal v, bool r) oper) => (-oper.v, oper.r);

    public (decimal v, bool r) Add((decimal v, bool r) left, (decimal v, bool r) right)
    {
        if (left.r != right.r)
            throw new InputErrorsException(lineNum, "addition with inconsistent value units");
        return (left.v + right.v, left.r);
    }

    public (decimal v, bool r) Sub((decimal v, bool r) left, (decimal v, bool r) right)
    {
        if (left.r != right.r)
            throw new InputErrorsException(lineNum, "subtraction with inconsistent value units");
        return (left.v - right.v, left.r);
    }

    public (decimal v, bool r) Mul((decimal v, bool r) left, (decimal v, bool r) right) => (left.v * right.v, left.r && right.r);

    public (decimal v, bool r) Div((decimal v, bool r) left, (decimal v, bool r) right)
    {
        try
        {
            var result = checked(left.v / right.v);
            return (result, left.r && right.r);
        }
        catch (DivideByZeroException)
        {
            throw new InputErrorsException(lineNum, "Transfer coefficient evaluation failed: divide by zero.");
        }
    }
}
