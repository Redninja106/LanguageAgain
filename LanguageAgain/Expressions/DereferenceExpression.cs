namespace LanguageAgain.Expressions;

internal record DereferenceExpression : Expression
{
    public Expression value { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        return ((InterpreterPointer)value.Evaluate(interpreter)!).value;
    }

    public override object? Assign(Interpreter interpreter, object value)
    {
        var ptr = (InterpreterPointer)this.value.Evaluate(interpreter)!;
        return ptr.value = value;
    }
}