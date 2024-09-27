namespace LanguageAgain.Expressions;

record VarDeclExpression(bool Immutable, Expression? TypeExpr, Token Name) : Expression
{
    public override object? Assign(Interpreter interpreter, object value)
    {
        interpreter.CreateVariable(Name.ToString(), value);
        return value;
    }
}

