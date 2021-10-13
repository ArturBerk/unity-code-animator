using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class ConstructorDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var fieldDeclarationSyntax = (ConstructorDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            nodeVisitor.Visit(fieldDeclarationSyntax.AttributeLists);
            // 1
            nodeVisitor.Visit(fieldDeclarationSyntax.Modifiers);
            // 2
            writer.WriteColored(fieldDeclarationSyntax.Identifier, context.Style.IdentifierColor);
            // 3
            nodeVisitor.Visit(fieldDeclarationSyntax.ParameterList);
            // 4
            nodeVisitor.Visit(fieldDeclarationSyntax.Initializer);
            // 5
            nodeVisitor.Visit(fieldDeclarationSyntax.Body);
            // 6
            nodeVisitor.Visit(fieldDeclarationSyntax.ExpressionBody);
            // 7
            writer.Write(fieldDeclarationSyntax.SemicolonToken);
        }
    }
}