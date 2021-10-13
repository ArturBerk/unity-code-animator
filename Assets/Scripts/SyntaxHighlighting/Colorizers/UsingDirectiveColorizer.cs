using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class UsingDirectiveColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var usingDirectiveSyntax = (UsingDirectiveSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            writer.WriteColored(usingDirectiveSyntax.UsingKeyword, context.Style.KeywordsColor);
            // 1
            writer.WriteColored(usingDirectiveSyntax.StaticKeyword, context.Style.KeywordsColor);
            // 2
            nodeVisitor.Visit(usingDirectiveSyntax.Alias);
            // 3
            nodeVisitor.Visit(usingDirectiveSyntax.Name);
            //
            writer.Write(usingDirectiveSyntax.SemicolonToken);
        }
    }
}