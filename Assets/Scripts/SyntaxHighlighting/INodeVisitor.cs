using Microsoft.CodeAnalysis;

namespace CodeAnimator
{
    interface INodeVisitor
    {
        void Visit(SyntaxNodeOrToken node);
    }

    static class NodeVisitorExtensions
    {
        public static void Visit<T>(this INodeVisitor visitor, SyntaxList<T> list) where T : SyntaxNode
        {
            foreach (var node in list)
            {
                visitor.Visit(node);
            }
        }
        
        public static void Visit(this INodeVisitor visitor, ChildSyntaxList list)
        {
            foreach (var node in list)
            {
                visitor.Visit(node);
            }
        }
        
        public static void Visit(this INodeVisitor visitor, SyntaxTokenList list)
        {
            foreach (var node in list)
            {
                visitor.Visit(node);
            }
        }
        
        public static void Visit(this INodeVisitor visitor, ChildSyntaxList list, int startIndex)
        {
            for (var index = startIndex; index < list.Count; index++)
            {
                var node = list[index];
                visitor.Visit(node);
            }
        }
        
        public static void Visit<T>(this INodeVisitor visitor, SeparatedSyntaxList<T> list) where T : SyntaxNode
        {
            for (var index = 0; index < list.Count; index++)
            {
                visitor.Visit(list[index]);
                if (index < list.SeparatorCount)
                    visitor.Visit(list.GetSeparator(index));
            }
        }
    }
}