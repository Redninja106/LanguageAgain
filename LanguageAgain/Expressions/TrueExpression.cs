namespace LanguageAgain.Expressions;

internal record TrueExpression : Expression
{
    public override object Evaluate(Interpreter interpreter)
    {
        return true;
    }
}