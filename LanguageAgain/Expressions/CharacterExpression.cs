namespace LanguageAgain.Expressions;

internal record CharacterExpression : Expression
{
    public Token character { get; set; }
}