namespace LanguageAgain.Expressions;

internal record NullExpression : Expression
{
    public object type { get; set; }
}