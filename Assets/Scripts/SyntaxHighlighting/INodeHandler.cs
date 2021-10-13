using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeAnimator
{
    interface INodeHandler<TContext>
    {
        void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, TContext context);
    }
}