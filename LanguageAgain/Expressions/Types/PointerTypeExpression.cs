using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions.Types;
internal record PointerTypeExpression : Expression
{
    public Expression ElementType;

    public override bool IsTypeExpression(Interpreter interpreter)
    {
        return true;
    }
}
