using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

namespace CodeAnimator
{
    public static class SyntaxHighlighter
    {
        private static Dictionary<Type, INodeHandler<ColorizingContext>> handlers = new Dictionary<Type, INodeHandler<ColorizingContext>>();

        static SyntaxHighlighter()
        {
            handlers.Add(typeof(NamespaceDeclarationSyntax), new NamespaceDeclarationColorizer());
            handlers.Add(typeof(ClassDeclarationSyntax), new TypeDeclarationColorizer());
            handlers.Add(typeof(InterfaceDeclarationSyntax), new TypeDeclarationColorizer());
            handlers.Add(typeof(StructDeclarationSyntax), new TypeDeclarationColorizer());
            handlers.Add(typeof(EnumDeclarationSyntax), new EnumDeclarationColorizer());
            handlers.Add(typeof(AttributeSyntax), new AttributeColorizer());
            handlers.Add(typeof(UsingDirectiveSyntax), new UsingDirectiveColorizer());
            handlers.Add(typeof(FieldDeclarationSyntax), new FieldDeclarationColorizer());
            handlers.Add(typeof(VariableDeclarationSyntax), new VariableDeclarationColorizer());
            handlers.Add(typeof(ConstructorDeclarationSyntax), new ConstructorDeclarationColorizer());
            handlers.Add(typeof(MethodDeclarationSyntax), new MethodDeclarationColorizer());
            handlers.Add(typeof(ParameterSyntax), new ParameterColorizer());
            handlers.Add(typeof(LiteralExpressionSyntax), new LiteralExpressionColorizer());
            handlers.Add(typeof(ReturnStatementColorizer), new ReturnStatementColorizer());
            handlers.Add(typeof(GenericNameSyntax), new GenericNameColorizer());
            handlers.Add(typeof(ObjectCreationExpressionSyntax), new ObjectCreationExpressionColorizer());
            handlers.Add(typeof(PropertyDeclarationSyntax), new PropertyDeclarationColorizer());
            handlers.Add(typeof(InvocationExpressionSyntax), new InvocationExpressionColorizer());
            handlers.Add(typeof(MemberAccessExpressionSyntax), new MemberAccessExpressionColorizer());
            
            handlers.Add(typeof(PredefinedTypeSyntax), new SimpleTokenColorizer(style => style.KeywordsColor));
            handlers.Add(typeof(TypeParameterSyntax), new TypeParameterColorizer());
            
            handlers.Add(typeof(ArrayTypeSyntax), new EnableIdentifierColorization());
            handlers.Add(typeof(BaseListSyntax), new EnableIdentifierColorization());
            handlers.Add(typeof(TypeOfExpressionSyntax), new EnableIdentifierColorization());
            handlers.Add(typeof(TypeArgumentListSyntax), new EnableIdentifierColorization());
            handlers.Add(typeof(TypeParameterConstraintClauseSyntax), new EnableIdentifierColorization());
            handlers.Add(typeof(TypeParameterConstraintSyntax), new EnableIdentifierColorization());
            
            handlers.Add(typeof(IdentifierNameSyntax), new IdentifierNameColorizer());
            handlers.Add(typeof(SyntaxToken), new SyntaxTokenColorizer());
        }

        public static string Highlight(CodeSyntaxTree codeSyntaxTree, Style style)
        {
            var syntaxTree = codeSyntaxTree;
            var root = syntaxTree.syntaxTree.GetCompilationUnitRoot();

            var context = new ColorizingContext(style);
            var visitor = new Visitor(context);
            visitor.Visit(root);

            //VisitNode(root, 0);
            
            return context.StringBuilder.ToString();
        }

        private static StringBuilder sb = new StringBuilder();
        
        private static void VisitNode(SyntaxNodeOrToken node, int depth)
        {
            var nodesAndTokens = node.ChildNodesAndTokens();
            var isLeaf = nodesAndTokens.Count == 0;
            var type = node.IsNode ? node.AsNode().GetType() : node.AsToken().GetType();
            sb.Clear();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("    ");
            }
            if (isLeaf)
            {
                Debug.Log("<color=red>" + sb.ToString() + type.Name + " - " + node.ToString() + "</color>");
                sb.Clear();
            }
            else
            {
                Debug.Log(sb.ToString() + type.Name);
                foreach (var syntaxNode in nodesAndTokens)
                {
                    VisitNode(syntaxNode, depth + 1);
                }
            }
        }

        private class Visitor : INodeVisitor
        {
            private readonly ColorizingContext context;

            public Visitor(ColorizingContext context)
            {
                this.context = context;
            }

            public void Visit(SyntaxNodeOrToken node)
            {
                if (node == null || node.IsMissing) return;
                var type = node.IsNode ? node.AsNode().GetType() : node.AsToken().GetType();
                if (handlers.TryGetValue(type, out var handler))
                {
                    handler.Handle(node, this, context);
                    return;
                }

                var childNodesAndTokens = node.ChildNodesAndTokens();
                if (childNodesAndTokens.Count == 0)
                {
                    // if (node.HasLeadingTrivia)
                    // {
                    //     context.StringBuilder.Append(node.GetLeadingTrivia());
                    // }
                    context.StringBuilder.Append(node.GetLeadingTrivia());
                    context.StringBuilder.Append(node.ToString());
                    context.StringBuilder.Append(node.GetTrailingTrivia());
                    // if (node.HasTrailingTrivia)
                    // {
                    //     context.StringBuilder.Append(node.GetTrailingTrivia());
                    // }
                }
                else
                {
                    
                    foreach (var childNodesAndToken in childNodesAndTokens)
                    {
                        Visit(childNodesAndToken);
                    }
                }
            }
        }
    }
}