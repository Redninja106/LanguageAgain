namespace LanguageAgain.Expressions;

internal record NumberExpression : Expression
{
    public Token Number { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        return int.Parse(Number.ToString());
    }
}