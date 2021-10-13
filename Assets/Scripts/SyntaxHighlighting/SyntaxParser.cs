using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnimator
{
    public static class SyntaxParser
    {
        public static CodeSyntaxTree Parse(this string text)
        {
            return new CodeSyntaxTree(text);
        }
        
        public static CodeSyntaxTree Parse(this StringBuilder text)
        {
            return new CodeSyntaxTree(new StringBuilderText(text, Encoding.Default, SourceHashAlgorithm.Sha1));
        }
    }
}