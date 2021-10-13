using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class VariableDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var variableDeclarationSyntax = (VariableDeclarationSyntax) node.AsNode();
            using (context.ColorizeIdentifierName)
            {
                nodeVisitor.Visit(variableDeclarationSyntax.Type);
            }
            nodeVisitor.Visit(variableDeclarationSyntax.Variables);
        }
    }
}