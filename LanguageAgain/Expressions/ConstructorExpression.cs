using LanguageAgain.Expressions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain.Expressions;
internal record ConstructorExpression : Expression
{
    public Expression Type;
    public Expression[] Expressions;

    public override object? Evaluate(Interpreter interpreter)
    {
        if (interpreter.ResolveType(Type) is StructTypeExpression structType)
        {
            Dictionary<string, object> values = [];

            foreach (var field in structType.fields)
            {
                var varDecl = (VarDeclExpression)field;
                values.Add(varDecl.Name.ToString(), null!);
            }

            foreach (var initializer in Expressions.Cast<BinaryExpression>())
            {
                if (initializer.op != TokenKind.Equal)
                    throw new();

                var name = (IdentifierExpression)initializer.left;
                values[name.identifier.ToString()] = initializer.right.Evaluate(interpreter)!;
            }

            return values;
        }

        if (interpreter.ResolveType(Type) is FunctionExpression fnExpr)
        {
            return new InterpreterFunction(fnExpr, Expressions);
        }

        return base.Evaluate(interpreter);
    }
}
