using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class PropertyDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var propertyDeclarationSyntax = (PropertyDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            nodeVisitor.Visit(propertyDeclarationSyntax.AttributeLists);
            // 1
            nodeVisitor.Visit(propertyDeclarationSyntax.Modifiers);
            using (context.ColorizeIdentifierName)
            {
                // 2
                nodeVisitor.Visit(propertyDeclarationSyntax.Type);
                // 3
                nodeVisitor.Visit(propertyDeclarationSyntax.ExplicitInterfaceSpecifier);
            }

            // 4
            nodeVisitor.Visit(propertyDeclarationSyntax.Identifier);
            // 5
            nodeVisitor.Visit(propertyDeclarationSyntax.AccessorList);
            // 6
            nodeVisitor.Visit(propertyDeclarationSyntax.ExpressionBody);
            // 7
            nodeVisitor.Visit(propertyDeclarationSyntax.Initializer);
            // 8
            nodeVisitor.Visit(propertyDeclarationSyntax.SemicolonToken);
        }
    }
}