namespace CodeAnimator
{
    public interface ILexicalRule
    {
        bool TryGetToken(string s, int startIndex, out Token token);
    }
}