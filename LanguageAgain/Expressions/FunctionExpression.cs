namespace LanguageAgain.Expressions;

internal record FunctionExpression : Expression
{
    public Expression callee { get; set; }
    public Expression[] arguments { get; set; }

    public override object? Evaluate(Interpreter interpreter)
    {
        var fn = (InterpreterFunction)callee.Evaluate(interpreter)!;
        return interpreter.RunFunction(fn, arguments.Select(a => a.Evaluate(interpreter)).ToArray()!);
    }
}