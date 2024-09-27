namespace LanguageAgain.Expressions;

abstract record Expression
{
    public virtual object? Evaluate(Interpreter interpreter)
    {
        throw new("not evaluatable");
    }

    public virtual object? Assign(Interpreter interpreter, object value)
    {
        throw new("not assignable");
    }

    public virtual bool IsTypeExpression(Interpreter interpreter)
    {
        return false;
    }

}
