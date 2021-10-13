using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class NamespaceDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var nameSpaceNode = (NamespaceDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            writer.WriteColored(nameSpaceNode.NamespaceKeyword, context.Style.KeywordsColor);
            // 1
            writer.WriteColored(nameSpaceNode.Name, context.Style.IdentifierColor);
            // 2 ...
            nodeVisitor.Visit(node.ChildNodesAndTokens(), 2);
        }
    }
}