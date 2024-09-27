namespace LanguageAgain.Expressions;

internal record SizeOfExpression : Expression
{
    public object type { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        return 0;
    }
}