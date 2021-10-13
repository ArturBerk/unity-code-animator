using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class AttributeColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var attributeSyntax = (AttributeSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            writer.WriteColored(attributeSyntax.Name, context.Style.IdentifierColor);
            // 1
            nodeVisitor.Visit(attributeSyntax.ArgumentList);
        }
    }
}