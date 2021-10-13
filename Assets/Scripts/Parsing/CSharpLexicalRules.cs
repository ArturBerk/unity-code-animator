using System.Text.RegularExpressions;

namespace CodeAnimator
{
    public static class CSharpLexicalRules
    {
        public readonly static LexicalRuleSet Set =new LexicalRuleSet(
            new RegexLexicalRule(new Regex(@"\G\s+"), TokenType.WhiteSpace),
            new RegexLexicalRule(new Regex(@"\G[\+\-\*\/\&\|\^\~\<\>\!]"), TokenType.Operator),
            new RegexLexicalRule(new Regex(@"\G((==)|(!=)|(<=)|(>=)|(<>)|(<<)|(>>)|(\/=)|(\*\*))"), TokenType.Operator),
            new RegexLexicalRule(new Regex(@"\G[()[]\{\}@,:=;.]"), TokenType.Delimiter),
            new RegexLexicalRule(new Regex("\\G\"[^\"]*\""), TokenType.String),
            new RegexLexicalRule(new Regex("\\G[_a-zA-Z][_a-zA-Z0-9]*"), TokenType.Identifier),
            new RegexLexicalRule(new Regex("\\G."), TokenType.Unknown)
        );
    }
}