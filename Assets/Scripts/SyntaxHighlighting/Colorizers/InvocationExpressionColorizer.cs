using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class InvocationExpressionColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            using (context.ColorizeMemberInvocation)
            {
                nodeVisitor.Visit(invocationExpression.Expression);
            }
            nodeVisitor.Visit(invocationExpression.ArgumentList);
        }
    }
}