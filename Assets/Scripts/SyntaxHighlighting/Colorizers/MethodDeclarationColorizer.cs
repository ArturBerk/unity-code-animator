using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class MethodDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var methodDeclarationSyntax = (MethodDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            // 0
            nodeVisitor.Visit(methodDeclarationSyntax.AttributeLists);
            // 1
            nodeVisitor.Visit(methodDeclarationSyntax.Modifiers);
            // 2
            using (context.ColorizeIdentifierName)
            {
                nodeVisitor.Visit(methodDeclarationSyntax.ReturnType);
                // 3
                nodeVisitor.Visit(methodDeclarationSyntax.ExplicitInterfaceSpecifier);
            }
            // 4
            writer.WriteColored(methodDeclarationSyntax.Identifier, context.Style.InvocationColor);
            // 5
            nodeVisitor.Visit(methodDeclarationSyntax.TypeParameterList);
            // 6
            nodeVisitor.Visit(methodDeclarationSyntax.ParameterList);
            // 7
            nodeVisitor.Visit(methodDeclarationSyntax.ConstraintClauses);
            // 8
            nodeVisitor.Visit(methodDeclarationSyntax.Body);
            // 9
            nodeVisitor.Visit(methodDeclarationSyntax.ExpressionBody);
            // 10
            writer.Write(methodDeclarationSyntax.SemicolonToken);
        }
    }
}