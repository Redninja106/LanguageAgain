namespace LanguageAgain.Expressions;

internal record MemberAccessExpression : Expression
{
    public Expression value { get; set; }
    public Token identifier { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        var obj = GetDict(interpreter);
        return obj[identifier.ToString()];
    }

    public override object? Assign(Interpreter interpreter, object value)
    {
        var obj = GetDict(interpreter);
        return obj[identifier.ToString()] = value;
    }

    private Dictionary<string, object> GetDict(Interpreter interpreter)
    {
        var obj = this.value.Evaluate(interpreter)!;

        while (obj is InterpreterPointer ptr) 
        {
            obj = ptr.value;
        }

        return (Dictionary<string, object>)obj;
    }
}