using LanguageAgain.Expressions;
using LanguageAgain.Expressions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LanguageAgain;
internal class Interpreter
{
    private readonly CompilationUnit compilationUnit;
    private readonly Dictionary<string, object> globals = [];
    private Stack<Dictionary<string, object>> stack = [];
    private readonly Dictionary<string, Expression> types = [];

    private InterpreterFunction print = new(null, null);

    public Interpreter(CompilationUnit compilationUnit)
    {
        this.compilationUnit = compilationUnit;

        InitGlobals();

        globals.Add("print", print);
    }

    public object? RunFunction(string name)
    {
        return RunFunction((InterpreterFunction)globals[name], []);
    }

    public object? RunFunction(InterpreterFunction function, object[] args)
    {
        if (function == print)
        {
            Console.WriteLine(args[0]);
            return null;
        }

        stack.Push([]);

        for (int i = 0; i < function.fnType.arguments.Length; i++)
        {
            var param = (VarDeclExpression)function.fnType.arguments[i];
            var arg = args[i];
            stack.Peek()[param.Name.ToString()] = arg;
        }

        foreach (var expr in function.Body)
        {
            if (expr is ReturnExpression)
            {
                var result = expr.Evaluate(this);
                stack.Pop();
                return result;
            }

            expr.Evaluate(this);
        }
        stack.Pop();
        return null;
    }

    public object? ResolveVariable(string identifier)
    {
        object? result;
        if (stack.TryPeek(out var scope) && scope.TryGetValue(identifier, out result))
        {
            return result;
        }
        if (globals.TryGetValue(identifier, out result))
        {
            return result;
        }
        throw new Exception($"unknown identifier '{identifier}'");
    }

    public void AssignVariable(string identifier, object value)
    {
        if (stack.TryPeek(out var scope) && scope.ContainsKey(identifier))
        {
            scope[identifier] = value;
            return;
        }
        if (globals.ContainsKey(identifier))
        {
            globals[identifier] = value;
            return;
        }
        throw new Exception($"unknown identifier '{identifier}'");
    }

    public Expression? ResolveType(Expression expr)
    {
        while (expr is IdentifierExpression ident)
        {
            if (types.TryGetValue(ident.identifier.ToString(), out Expression? e))
            {
                expr = e;
            }
            else
            {
                return null;
            }
        }

        return expr;
    }

    private void InitGlobals()
    {
        foreach (var expression in compilationUnit.Expressions)
        {
            if (expression is BinaryExpression bin && bin.op == TokenKind.Equal)
            {
                if (bin.left is not VarDeclExpression varDecl)
                {
                    throw new();
                }

                if (varDecl.TypeExpr is TypeExpression || bin.right.IsTypeExpression(this))
                {
                    types.Add(varDecl.Name.ToString(), bin.right);
                }
                else
                {
                    globals.Add(varDecl.Name.ToString(), bin.right.Evaluate(this));
                }
            }
            else
            {
                throw new();
            }
        }
    }

    public void CreateVariable(string name, object variable)
    {
        stack.Peek()[name] = variable;
    }
}

class Scope
{

}
