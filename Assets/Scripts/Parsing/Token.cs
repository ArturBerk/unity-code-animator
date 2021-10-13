namespace CodeAnimator
{
    public readonly struct Token
    {
        public readonly TokenType Type;
        public readonly int StartIndex;
        public readonly int Length;
        
        public Token(int startIndex, int length, TokenType type)
        {
            StartIndex = startIndex;
            Length = length;
            Type = type;
        }
    }

    public static class ParsingExtensions
    {
        public static string ReadTokenValue(ref this Token token, string s)
        {
            return s.Substring(token.StartIndex, token.Length);
        }
    }
}