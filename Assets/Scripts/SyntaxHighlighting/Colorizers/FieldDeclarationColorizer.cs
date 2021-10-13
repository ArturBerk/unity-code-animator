using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class FieldDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var fieldDeclarationSyntax = (FieldDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            nodeVisitor.Visit(fieldDeclarationSyntax.AttributeLists);
            // 1
            nodeVisitor.Visit(fieldDeclarationSyntax.Modifiers);
            // 2
            nodeVisitor.Visit(fieldDeclarationSyntax.Declaration);
            // 3
            writer.Write(fieldDeclarationSyntax.SemicolonToken);
        }
    }
}