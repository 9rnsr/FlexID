using System.Globalization;
using Sprache;
using static Sprache.Parse;

namespace FlexID;

/// <summary>
/// インプットにおける文法とその解析処理を提供する。
/// </summary>
/// <typeparam name="T">解析結果を表現する型。</typeparam>
public class InputParser<T>
{
    public Parser<string> Identifier { get; }

    public Parser<T> VarExpr { get; }

    public Parser<T> NumberExpr { get; }

    public Parser<T> StringExpr { get; }

    public Parser<T> Expr { get; }

    public Parser<(string ident, T expr)> VarDecl { get; }

    public Parser<T> Coefficient { get; }

    public Parser<T> Compartment { get; }

    public InputParser(Visitor<T> visitor)
    {
        static Parser<string> OrEmpty<X>(Parser<X> parser) =>
            parser.Optional().Select(x => x.IsDefined ? x.Get()!.ToString()! : "");

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

        StringExpr =
            from quote in Chars("\'\"")
            from content in CharExcept(['\\', quote]).Or(Char('\\').Then(_ => AnyChar)).Many().Text()
            from _end in Char(quote)
            select visitor.String(content);

        var Parenthesis = Ref(() => Expr).Contained(Char('('), Char(')'));

        var Primary = VarExpr.Or(NumberExpr).Or(StringExpr).Or(Parenthesis);

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
            from expr in Char('$').Then(_ => VarExpr.Or(Parenthesis)).Or(SignedNumerExpr)
            select expr;

        var compartmentName =
            from name in CharExcept(' ').AtLeastOnce().Text()
            select visitor.String(name);

        Compartment =
            from expr in Char('$').Then(_ => VarExpr.Or(Parenthesis)).Or(compartmentName)
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
    T String(string value);
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

public record class AbstractValue { }

public record class NumberValue(decimal Value, bool IsFrac) : AbstractValue;

public record class StringValue(string Content) : AbstractValue;

/// <summary>
/// インプット上の変数定義や部分式の評価を行う。
/// </summary>
public class InputEvaluator : Visitor<AbstractValue>
{
    private readonly string nuc;

    private readonly InputErrors errors;

    private readonly InputParser<AbstractValue> parser;

    private readonly Dictionary<string, AbstractValue> variables = [];

    private Location loc;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputEvaluator(string nuc, InputErrors errors)
    {
        this.nuc = nuc;
        this.errors = errors;
        this.parser = new InputParser<AbstractValue>(this);
    }

    /// <summary>
    /// 変数の定義行の場合にこれの解釈を行う。
    /// </summary>
    /// <param name="loc">位置情報。</param>
    /// <param name="varName">変数名。</param>
    /// <param name="varValue">変数の定義値を表す文字列。</param>
    /// <returns>変数の定義に成功した場合は<see langword="true"/>を返す。</returns>
    public bool TryReadVarDecl(Location loc, string varName, string varValue)
    {
        this.loc = loc;

        var resultId = parser.Identifier.Token().End().TryParse(varName);
        var resultIz = parser.Expr.Token().End().TryParse(varValue);
        if (!resultId.WasSuccessful || !resultIz.WasSuccessful)
            return false;

        var ident = resultId.Value;
        var initializer = resultIz.Value;

        // 変数定義の上書きに対してエラーを報告する。
        if (variables.ContainsKey(ident))
        {
            errors.AddError(loc, $"Variable '{ident}' is already defined for nuclide '{nuc}'.");
            return false;
        }

        variables[ident] = initializer;

        return true;
    }

    public bool TryReadCoefficient(Location loc, string input, out (decimal value, bool isFrac) result)
    {
        this.loc = loc;
        result = default;

        IResult<AbstractValue> r;
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
            errors.AddError(loc, $"Transfer coefficient evaluation failed: {ex.Message}.");
            return false;
        }
        if (!r.WasSuccessful || r.Value is not NumberValue n)
        {
            errors.AddError(loc, $"Transfer coefficient should be evaluated to a number, not '{input}'.");
            return false;
        }

        //Debug.WriteLine($"Line {loc.LineNum} '{input}' ==> {n.Value * (n.IsFrac ? 100 : 1)}{(n.IsFrac ? "%" : "")}");
        result = (n.Value, n.IsFrac);
        return true;
    }

    public bool TryReadCompartment(Location loc, ref string target)
    {
        this.loc = loc;

        IResult<AbstractValue> r;
        try
        {
            r = parser.Compartment.Token().End().TryParse(target);
        }
        catch (InputErrorsException ex)
        {
            errors.AddErrors(ex);
            return false;
        }
        if (!r.WasSuccessful || r.Value is not StringValue s)
        {
            errors.AddError(loc, $"Expected a compartment name, not '{target}'.");
            return false;
        }
        target = s.Content;
        return true;
    }

    public AbstractValue Var(string ident)
    {
        // 定義されていない変数の使用に対してエラーを報告する。
        if (!variables.TryGetValue(ident, out var v))
            throw new InputErrorsException(loc, $"Undefined variable '{ident}' for nuclide '{nuc}'.");
        return v;
    }

    public AbstractValue Number(string input, string unit)
    {
        var value = decimal.Parse(input, NumberStyles.Float);
        var isFrac = (unit == "%");
        if (isFrac)
            value = value / 100;
        return new NumberValue(value, isFrac);
    }

    public AbstractValue String(string value)
    {
        return new StringValue(value);
    }

    public AbstractValue Pos(AbstractValue oper) => oper switch
    {
        NumberValue n => n,
        _ => throw new InputErrorsException(loc, "Unexpected operands"),
    };

    public AbstractValue Neg(AbstractValue oper) => oper switch
    {
        NumberValue n => new NumberValue(-n.Value, n.IsFrac),
        _ => throw new InputErrorsException(loc, "Unexpected operands"),
    };

    public AbstractValue Add(AbstractValue left, AbstractValue right)
    {
        return (left, right) switch
        {
            (NumberValue a, NumberValue b) =>
                a.IsFrac == b.IsFrac ? new NumberValue(a.Value + b.Value, a.IsFrac)
                                     : throw new InputErrorsException(loc, "Addition with inconsistent value units"),
            (StringValue a, StringValue b) => new StringValue(a.Content + b.Content),
            _ => throw new InputErrorsException(loc, "Unexpected operands"),
        };
    }

    public AbstractValue Sub(AbstractValue left, AbstractValue right)
    {
        return (left, right) switch
        {
            (NumberValue a, NumberValue b) =>
                a.IsFrac == b.IsFrac ? new NumberValue(a.Value - b.Value, a.IsFrac)
                                     : throw new InputErrorsException(loc, "Subtraction with inconsistent value units"),
            _ => throw new InputErrorsException(loc, "Unexpected operands"),
        };
    }

    public AbstractValue Mul(AbstractValue left, AbstractValue right) => (left, right) switch
    {
        (NumberValue a, NumberValue b) => new NumberValue(a.Value * b.Value, a.IsFrac && b.IsFrac),
        _ => throw new InputErrorsException(loc, "Unexpected operands"),
    };

    public AbstractValue Div(AbstractValue left, AbstractValue right)
    {
        if ((left, right) is (NumberValue a, NumberValue b))
        {
            try
            {
                var result = checked(a.Value / b.Value);
                return new NumberValue(result, a.IsFrac && b.IsFrac);
            }
            catch (DivideByZeroException)
            {
                throw new InputErrorsException(loc, "Transfer coefficient evaluation failed: divide by zero.");
            }
        }
        else
        {
            throw new InputErrorsException(loc, "Unexpected operands");
        }
    }
}
