using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class GenericNameColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var typeOfExpressionSyntax = (GenericNameSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);

            // var notColorize = typeOfExpressionSyntax.Ancestors().Any(c => c is MemberAccessExpressionSyntax);
            // // 0
            // if (notColorize)
            // {
            //     writer.Write(typeOfExpressionSyntax.Identifier);
            // }
            // else
            // {
            //     writer.WriteColored(typeOfExpressionSyntax.Identifier, context.Style.IdentifierColor);
            // }
            
            if (context.IsColorizeIdentifierName)
            {
                writer.WriteColored(typeOfExpressionSyntax.Identifier, context.Style.IdentifierColor);
            }
            else if (context.IsColorizeMemberInvocation)
            {
                writer.WriteColored(typeOfExpressionSyntax.Identifier, context.Style.InvocationColor);
                context.ResetColorizeMemberInvocation();
            }
            else
            {
                writer.Write(typeOfExpressionSyntax.Identifier);
            }
            
            
            // 1
            nodeVisitor.Visit(typeOfExpressionSyntax.TypeArgumentList);
        }
    }
}