using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record ConditionalExpression : Expression
{
    public Expression Condition;
    public Expression True;
    public Expression False;
}
