using System;
using Microsoft.CodeAnalysis;
using UnityEngine;

namespace CodeAnimator
{
    class SimpleTokenColorizer : INodeHandler<ColorizingContext>
    {
        private readonly Func<Style, string> colorGetter;

        public SimpleTokenColorizer(Func<Style, string> colorGetter)
        {
            this.colorGetter = colorGetter;
        }

        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            writer.WriteColored(node, colorGetter(context.Style));
        }
    }
}