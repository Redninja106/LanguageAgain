namespace LanguageAgain.Expressions;

internal record ArrayIndexExpression : Expression
{
    public Expression array { get; set; }
    public Expression index { get; set; }
}