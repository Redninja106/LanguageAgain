namespace LanguageAgain.Expressions;

internal record StringExpression : Expression
{
    public Token value { get; set; }

    public override object Evaluate(Interpreter interpreter)
    {
        return value.ToString()[1..^1];
    }
}