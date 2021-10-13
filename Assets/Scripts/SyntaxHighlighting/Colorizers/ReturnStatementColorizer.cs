using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class ReturnStatementColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var returnStatementSyntax = (ReturnStatementSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            writer.WriteColored(returnStatementSyntax.ReturnKeyword, context.Style.KeywordsColor);
            // 1
            nodeVisitor.Visit(returnStatementSyntax.Expression);
            // 2
            writer.Write(returnStatementSyntax.SemicolonToken);
        }
    }
}