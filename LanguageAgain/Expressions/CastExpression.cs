namespace LanguageAgain.Expressions;

internal record CastExpression : Expression
{
    public Expression value { get; set; }
    public object type { get; set; }
}