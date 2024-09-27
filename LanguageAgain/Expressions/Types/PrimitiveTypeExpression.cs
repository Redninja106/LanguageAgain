using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions.Types;
internal record PrimitiveTypeExpression : Expression
{
    public Token KeywordExpression;

    public override bool IsTypeExpression(Interpreter interpreter)
    {
        return true;
    }
}
