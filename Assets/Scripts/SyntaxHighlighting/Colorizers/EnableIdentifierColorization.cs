using Microsoft.CodeAnalysis;

namespace CodeAnimator
{
    class EnableIdentifierColorization : INodeHandler<ColorizingContext>
    {
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            using (context.ColorizeIdentifierName)
            {
                nodeVisitor.Visit(node.ChildNodesAndTokens());
            }
        }
    }
}