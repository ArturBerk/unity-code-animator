using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class ObjectCreationExpressionColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var objectCreationExpressionSyntax = (ObjectCreationExpressionSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            writer.WriteColored(objectCreationExpressionSyntax.NewKeyword, context.Style.KeywordsColor);
            using (context.ColorizeIdentifierName)
            {
                // 1
                nodeVisitor.Visit(objectCreationExpressionSyntax.Type);
            }
            // 2
            nodeVisitor.Visit(objectCreationExpressionSyntax.ArgumentList);
            // 3
            nodeVisitor.Visit(objectCreationExpressionSyntax.Initializer);
        }
    }
}