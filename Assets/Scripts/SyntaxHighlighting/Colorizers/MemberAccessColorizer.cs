using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class MemberAccessExpressionColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var expression = (MemberAccessExpressionSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);

            var was = context.IsColorizeMemberInvocation;
            context.ResetColorizeMemberInvocation();
            
            nodeVisitor.Visit(expression.Expression);
            
            writer.Write(expression.OperatorToken);

            if (was)
            {
                using (context.ColorizeMemberInvocation)
                {
                    nodeVisitor.Visit(expression.Name);
                }
            }
            else
            {
                nodeVisitor.Visit(expression.Name);
            }
        }
    }
}