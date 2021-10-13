using System;
using System.Text.RegularExpressions;

namespace CodeAnimator
{
    public class RegexLexicalRule : ILexicalRule
    {
        private readonly Regex regex;
        private readonly TokenType tokenType;

        public RegexLexicalRule(Regex regex, TokenType tokenType)
        {
            this.regex = regex;
            this.tokenType = tokenType;
        }

        public bool TryGetToken(string s, int startIndex, out Token token)
        {
            var match = regex.Match(s, startIndex);
            if (match.Success)
            {
                if (match.Length == 0)
                {
                    throw new Exception(string.Format("Regex match length is zero. This can lead to infinite loop. " +
                                                      "Please modify your regex {0} for {1} so that it can't matche character of zero length", regex, tokenType));
                }
                token = new Token(match.Index, match.Length, tokenType);
                return true;
            }

            token = default;
            return false;
        }
    }
}