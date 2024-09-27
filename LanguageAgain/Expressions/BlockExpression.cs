using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record BlockExpression : Expression
{
    public Expression[] Expressions;
    public Expression? ResultExpression;
}
