using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class TypeParameterColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var typeParameterSyntax = (TypeParameterSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            
            // 0
            nodeVisitor.Visit(typeParameterSyntax.AttributeLists);
            // 1
            writer.WriteColored(typeParameterSyntax.VarianceKeyword, context.Style.KeywordsColor);
            // 2
            writer.WriteColored(typeParameterSyntax.Identifier, context.Style.IdentifierColor);
        }
    }
}