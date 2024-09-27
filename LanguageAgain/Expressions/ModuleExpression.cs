using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record ModuleExpression : Expression
{
    public Token[] Names;

    public override object? Evaluate(Interpreter interpreter)
    {
        return base.Evaluate(interpreter);
    }
}
