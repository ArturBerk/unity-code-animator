using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class ParameterColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var parameterSyntax = (ParameterSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            
            // 0
            nodeVisitor.Visit(parameterSyntax.AttributeLists);
            // 1
            nodeVisitor.Visit(parameterSyntax.Modifiers);
            // 2
            using (context.ColorizeIdentifierName)
            {
                nodeVisitor.Visit(parameterSyntax.Type);
            }
            // 3
            writer.Write(parameterSyntax.Identifier);
            // 4
            nodeVisitor.Visit(parameterSyntax.Default);
        }
    }
}