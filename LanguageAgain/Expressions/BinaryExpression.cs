namespace LanguageAgain.Expressions;

internal record class BinaryExpression : Expression
{
    public TokenKind op;
    public Expression left;
    public Expression right;

    public override object Evaluate(Interpreter interpreter)
    {
        if (op == TokenKind.Equal)
        {
            return left.Assign(interpreter, right.Evaluate(interpreter));
        }

        object l = left.Evaluate(interpreter);
        object r = right.Evaluate(interpreter);

        if (l is int v1 && r is int v2)
        {
            switch (op)
            {
                case TokenKind.Plus:
                    return v1 + v2;
                case TokenKind.Minus:
                    return v1 - v2;
            }
        }

        throw new();
    }
}

