using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record NamespaceAccessExpression : Expression
{
    public override object? Evaluate(Interpreter interpreter)
    {
        return base.Evaluate(interpreter);
    }
}
