using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record ReturnExpression : Expression
{
    public Expression? ReturnValue;

    public override object? Evaluate(Interpreter interpreter)
    {
        return ReturnValue?.Evaluate(interpreter);
    }
}
