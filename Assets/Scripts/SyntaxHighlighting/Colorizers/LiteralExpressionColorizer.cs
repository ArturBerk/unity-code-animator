using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnimator
{
    class LiteralExpressionColorizer : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var expressionSyntax = (LiteralExpressionSyntax) node;
            var token = expressionSyntax.Token;
            var tokenText = token.Text;
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            if (tokenText.StartsWith("\""))
            {
                writer.WriteColored(token, context.Style.StringLiteralColor);
                return;
            }
            switch (tokenText)
            {
                case "default":
                case "true":
                case "false":
                case "null":
                    writer.WriteColored(token, context.Style.KeywordsColor);
                    break;
                default:
                    writer.WriteColored(token, context.Style.LiteralColor);
                    break;
            }
        }
    }
}