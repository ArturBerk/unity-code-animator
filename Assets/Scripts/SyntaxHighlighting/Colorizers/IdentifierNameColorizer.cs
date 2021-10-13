using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class IdentifierNameColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            
            var token = identifierNameSyntax.Identifier.Text;
            if (token == "var")
            {
                writer.WriteColored(identifierNameSyntax.Identifier, context.Style.KeywordsColor);
            }
            else if (context.IsColorizeIdentifierName)
            {
                writer.WriteColored(identifierNameSyntax.Identifier, context.Style.IdentifierColor);
            }
            else if (context.IsColorizeMemberInvocation)
            {
                writer.WriteColored(identifierNameSyntax.Identifier, context.Style.InvocationColor);
                context.ResetColorizeMemberInvocation();
            }
            else
            {
                writer.Write(identifierNameSyntax.Identifier);
            }
        }
    }
}