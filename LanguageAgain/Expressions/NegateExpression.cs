namespace LanguageAgain.Expressions;

internal record NegateExpression : Expression
{
    public Expression value { get; set; }
}