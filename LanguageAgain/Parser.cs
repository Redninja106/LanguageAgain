using LanguageAgain.Expressions;
using LanguageAgain.Expressions.Types;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAgain;
internal static class Parser
{
//    static Expression ParseExpression(TokenReader reader)
//    {
//        if (reader.Next(TokenKind.Let))
//        {
//            return ParseVarDecl(true, reader);
//        }

//        if (reader.Next(TokenKind.Var))
//        {
//            return ParseVarDecl(false, reader);
//        }
//    }

//    static Expression ParseWithPrecedence(TokenReader reader, Precedence precedence)
//    {
//        if (precedence > Precedence.VarDecl)
//        {
//            if (reader.Next(TokenKind.Let))
//            {
//                return ParseVarDecl(true, reader);
//            }

//            if (reader.Next(TokenKind.Var))
//            {
//                return ParseVarDecl(false, reader);
//            }
//        }

//        if (precedence > Precedence.Constructor)
//        {
//            var type = ParseWithPrecedence(reader, Precedence.Constructor);
//            reader.NextOrError(TokenKind.OpenBracket);
//            reader.NextOrError(TokenKind.CloseBracket);
//        }
//    }

    //private static Expression ParseRestWithPrecedence(TokenReader reader, Expression left, Precedence precedence)
    //{
    //    while (true)
    //    {
    //        if (reader.ReadLineTerminator())
    //        {
    //            return left;
    //        }

    //        TokenKind op = reader.Current.kind;
    //        Precedence opPrecedence = GetOperatorPrecedence(op);
    //        if (opPrecedence is Precedence.None)
    //        {
    //            break;
    //        }

    //        bool isLeftAssociative = precedence != Precedence.Assignment;
    //        if (opPrecedence < precedence || (isLeftAssociative && opPrecedence == precedence))
    //        {
    //            break;
    //        }

    //        // consume the operator
    //        reader.Next();

    //        var right = ParseWithPrecedence(reader, opPrecedence);
    //        left = new BinaryExpression() { op = op, left = left, right = right };
    //    }

    //    return left;
    //}

    public static CompilationUnit ParseCompilationUnit(TokenReader reader)
    {
        List<Expression> expressions = [];
        while (reader.Peek().kind != TokenKind.EndOfSource)
        {
            expressions.Add(ParseExpression(reader));
        }
        return new(expressions.ToArray());
    }

    public static Expression ParseExpression(TokenReader reader)
    {
        return ParseWithPrecedence(reader, Precedence.None);
    }

    public static Expression ParseWithPrecedence(TokenReader reader, Precedence precedence)
    {
        return ParseRestWithPrecedence(reader, ParseTerm(reader), precedence);
    }

    private static Expression ParseRestWithPrecedence(TokenReader reader, Expression left, Precedence precedence)
    {
        while (true)
        {
            TokenKind op = reader.Current.kind;
            Precedence opPrecedence = GetOperatorPrecedence(op);
            if (opPrecedence is Precedence.None)
            {
                break;
            }

            bool isLeftAssociative = precedence != Precedence.Assignment;
            if (opPrecedence < precedence || (isLeftAssociative && opPrecedence == precedence))
            {
                break;
            }

            // consume the operator
            reader.Next();

            var right = ParseWithPrecedence(reader, opPrecedence);
            left = new BinaryExpression() { op = op, left = left, right = right };
        }

        return left;
    }

    private static Expression ParseTerm(TokenReader reader)
    {
        // parse prefix ops recursively
        if (reader.Next(TokenKind.Star))
        {
            return new DereferenceExpression() { value = ParseTerm(reader) };
        }
        if (reader.Next(TokenKind.And))
        {
            return new AddressOfExpression() { value = ParseTerm(reader) };
        }
        if (reader.Next(TokenKind.Minus))
        {
            return new NegateExpression() { value = ParseTerm(reader) };
        }

        var term = ParseTermOnly(reader);

        // parse any number of postfix ops
        List<Expression> arguments = [];
        while (true)
        {
            if (reader.Next(TokenKind.OpenParenthesis))
            {
                term = new Expressions.FunctionExpression 
                { 
                    callee = term, 
                    arguments = ParseExpressionList(reader, TokenKind.CloseParenthesis),
                };
                continue;
            }
            else if (reader.Next(TokenKind.OpenBracket))
            {
                var idx = ParseExpression(reader);
                reader.NextOrError(TokenKind.CloseBracket);

                term = new ArrayIndexExpression { array = term, index = idx };
                continue;
            }
            else if (reader.Next(TokenKind.OpenBrace))
            {
                term = new ConstructorExpression()
                {
                    Type = term,
                    Expressions = ParseExpressionList(reader, TokenKind.CloseBrace, TokenKind.Semicolon),
                };
            }
            else if (reader.Next(TokenKind.Dot))
            {
                var identifier = reader.NextOrError(TokenKind.Identifier);
                term = new MemberAccessExpression { value = term, identifier = identifier };
                continue;
            }
            else if (reader.Next(TokenKind.As))
            {
                var type = ParseExpression(reader);
                term = new CastExpression { value = term, type = type };
            }
            else if (reader.Next(TokenKind.Star))
            {
                term = new PointerTypeExpression() { ElementType = term };
            }
            else
            {
                break;
            }
        }

        return term;
    }

    private static Expression[] ParseExpressionList(TokenReader reader, TokenKind terminator, TokenKind delimiter = TokenKind.Comma)
    {
        List<Expression> arguments = [];
        if (!reader.Next(terminator))
        {
            while (true)
            {
                arguments.Add(ParseExpression(reader));

                if (reader.Next(terminator))
                {
                    break;
                }
                else if (reader.Next(delimiter))
                {
                    if (reader.Next(terminator))
                    {
                        break;
                    }

                    continue;
                }
                else
                {
                    // will throw as we've already confirmed terminator delimiter is missing
                    // we do it this way to get the proper message
                    reader.NextOrError(delimiter);
                }
            }
        }

        return arguments.ToArray();
    }

    private static Expression ParseTermOnly(TokenReader reader)
    {
        // terms
        if (reader.Next(TokenKind.OpenParenthesis))
        {
            var result = ParseWithPrecedence(reader, Precedence.None);
            reader.NextOrError(TokenKind.CloseParenthesis);
            return result;
        }
        if (reader.Next(TokenKind.Number, out Token number))
        {
            return new NumberExpression { Number = number };
        }
        if (reader.Next(TokenKind.Identifier, out Token identifier))
        {
            return new IdentifierExpression { identifier = identifier };
        }
        if (reader.Next(TokenKind.Character, out Token character))
        {
            return new CharacterExpression { character = character };
        }
        if (reader.Next(TokenKind.String, out Token str))
        {
            return new StringExpression { value = str };
        }
        if (reader.Next(TokenKind.True))
        {
            return new TrueExpression();
        }
        if (reader.Next(TokenKind.False))
        {
            return new FalseExpression();
        }
        if (reader.Next(TokenKind.Null))
        {
            reader.NextOrError(TokenKind.OpenParenthesis);
            var type = ParseExpression(reader);
            reader.NextOrError(TokenKind.CloseParenthesis);
            return new NullExpression { type = type };
        }
        if (reader.Next(TokenKind.SizeOf))
        {
            reader.NextOrError(TokenKind.OpenParenthesis);
            var type = ParseExpression(reader);
            reader.NextOrError(TokenKind.CloseParenthesis);
            return new SizeOfExpression { type = type };
        }
        if (reader.Next(TokenKind.Let))
        {
            return ParseVarDecl(true, reader);
        }
        if (reader.Next(TokenKind.Var))
        {
            return ParseVarDecl(false, reader);
        }
        Token kwTok;
        if (reader.Next(TokenKind.Int, out kwTok) ||
            reader.Next(TokenKind.Void, out kwTok)
            )
        {
            return new PrimitiveTypeExpression()
            {
                KeywordExpression = kwTok,
            };
        }
        if (reader.Next(TokenKind.Return))
        {
            return new ReturnExpression() { ReturnValue = ParseExpression(reader) };
        }
        if (reader.Next(TokenKind.Type))
        {
            return new TypeExpression();
        }
        if (reader.Next(TokenKind.Struct))
        {
            reader.NextOrError(TokenKind.OpenBrace);
            Expression[] exprs = ParseExpressionList(reader, TokenKind.CloseBrace, TokenKind.Semicolon);
            // reader.NextOrError(TokenKind.CloseBrace);
            return new StructTypeExpression() { fields = exprs };
        }

        throw new Exception($"unexpected token {reader.Current.kind} ({reader.Current})");
    }

    public static Precedence GetOperatorPrecedence(TokenKind op)
    {
        switch (op)
        {
            case TokenKind.Minus:
            case TokenKind.Plus:
                return Precedence.Additive;
            case TokenKind.Star:
            case TokenKind.Slash:
            case TokenKind.Percent:
                return Precedence.Multiplicative;
            case TokenKind.Equal:
                return Precedence.Assignment;
            case TokenKind.EqualEqual:
            case TokenKind.NotEqual:
            case TokenKind.LessThan:
            case TokenKind.LessThanEqual:
            case TokenKind.GreaterThan:
            case TokenKind.GreaterThanEqual:
                return Precedence.Equality;
            case TokenKind.OrOr:
                return Precedence.LogicalOr;
            case TokenKind.AndAnd:
                return Precedence.LogicalAnd;
            default:
                return Precedence.None;
        }
    }

    static VarDeclExpression ParseVarDecl(bool immutable, TokenReader reader)
    {
        Expression? type = null;
        if (reader.Current.kind != TokenKind.Identifier || reader.Peek().kind is not (TokenKind.Equal or TokenKind.Semicolon))
        {
            // no type provided
            type = ParseWithPrecedence(reader, Precedence.VarDecl);
        }

        var name = reader.NextOrError(TokenKind.Identifier);

        return new VarDeclExpression(immutable, type, name);
    }
}
