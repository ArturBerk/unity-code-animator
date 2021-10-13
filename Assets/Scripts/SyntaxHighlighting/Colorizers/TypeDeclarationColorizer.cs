using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class TypeDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var classNode = (TypeDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);

            // 0
            nodeVisitor.Visit(classNode.AttributeLists);
            // 1
            nodeVisitor.Visit(classNode.Modifiers);
            // 2
            writer.WriteColored(classNode.Keyword, context.Style.KeywordsColor);
            // 3
            writer.WriteColored(classNode.Identifier, context.Style.IdentifierColor);
            // 4
            nodeVisitor.Visit(classNode.TypeParameterList);
            // 5
            nodeVisitor.Visit(classNode.BaseList);
            // 6
            nodeVisitor.Visit(classNode.ConstraintClauses);
            // 7
            writer.Write(classNode.OpenBraceToken);
            // 8
            nodeVisitor.Visit(classNode.Members);
            // 9
            writer.Write(classNode.CloseBraceToken);
            // 10
            writer.Write(classNode.SemicolonToken);
        }
    }
}