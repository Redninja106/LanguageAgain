namespace LanguageAgain.Expressions;

internal record IdentifierExpression : Expression
{
    public Token identifier { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        return interpreter.ResolveVariable(identifier.ToString());
    }

    public override object? Assign(Interpreter interpreter, object value)
    {
        interpreter.AssignVariable(identifier.ToString(), value);
        return value;
    }

    public override bool IsTypeExpression(Interpreter interpreter)
    {
        return interpreter.ResolveType(this) != null;
    }
}