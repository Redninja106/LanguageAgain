namespace LanguageAgain.Expressions;

internal record AddressOfExpression : Expression
{
    public Expression value { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        return new InterpreterPointer(value.Evaluate(interpreter));
    }
}