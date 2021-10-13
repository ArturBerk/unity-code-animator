using Microsoft.CodeAnalysis;
using UnityEngine;

namespace CodeAnimator
{
    delegate bool PredicateTokenDelegate(SyntaxNodeOrToken node, ColorizingContext context, out string color);
    
    class PredicateTokenColorizer : INodeHandler<ColorizingContext>
    {
        private readonly PredicateTokenDelegate predicate;

        public PredicateTokenColorizer(PredicateTokenDelegate predicate)
        {
            this.predicate = predicate;
        }

        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            if (predicate(node, context, out var color))
            {
                writer.WriteColored(node, color);
            }
            else
            {
                nodeVisitor.Visit(node.ChildNodesAndTokens());
            }
        }
    }
}