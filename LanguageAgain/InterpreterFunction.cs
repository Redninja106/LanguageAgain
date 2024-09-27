using LanguageAgain.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain;
internal record InterpreterFunction(FunctionExpression fnType, Expression[] Body)
{
}
