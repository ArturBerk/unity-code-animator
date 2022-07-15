using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnimator
{
    public class CodeSyntaxTree
    {
        internal SyntaxTree syntaxTree;

        public CodeSyntaxTree(string code)
        {
            syntaxTree = CSharpSyntaxTree.ParseText(code,
                CSharpParseOptions.Default.WithPreprocessorSymbols("UNITY_EDITOR"));
        }

        public CodeSyntaxTree(SourceText code)
        {
            syntaxTree = CSharpSyntaxTree.ParseText(code,
                CSharpParseOptions.Default.WithPreprocessorSymbols("UNITY_EDITOR"));
        }
    }
}