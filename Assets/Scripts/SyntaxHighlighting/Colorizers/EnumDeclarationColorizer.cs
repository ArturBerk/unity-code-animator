using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class EnumDeclarationColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var classNode = (EnumDeclarationSyntax) node.AsNode();
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            using (context.ColorizeIdentifierName)
            {
                // 0
                nodeVisitor.Visit(classNode.AttributeLists);
                // 1
                nodeVisitor.Visit(classNode.Modifiers);
                // 2
                writer.WriteColored(classNode.EnumKeyword, context.Style.KeywordsColor);
                // 3
                writer.WriteColored(classNode.Identifier, context.Style.IdentifierColor);
                // 4
                nodeVisitor.Visit(classNode.BaseList);
                // 5
                writer.Write(classNode.OpenBraceToken);
                // 6
                nodeVisitor.Visit(classNode.Members);
                // 7
                writer.Write(classNode.CloseBraceToken);
                // 8
                writer.Write(classNode.SemicolonToken);
            }
        }
    }
}