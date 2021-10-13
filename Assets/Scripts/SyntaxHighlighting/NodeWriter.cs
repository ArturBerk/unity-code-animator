using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnimator
{
    readonly struct NodeWriter : IDisposable
    {
        private readonly SyntaxNodeOrToken syntaxNodeOrToken;
        private readonly StringBuilder codeBuilder;
        private readonly ColorizingContext context;

        public NodeWriter(SyntaxNodeOrToken syntaxNodeOrToken, StringBuilder codeBuilder, ColorizingContext context)
        {
            this.syntaxNodeOrToken = syntaxNodeOrToken;
            this.codeBuilder = codeBuilder;
            this.context = context;
            // if (syntaxNodeOrToken.HasLeadingTrivia) 
            //     codeBuilder.Append(syntaxNodeOrToken.GetLeadingTrivia().ToFullString());
        }
        
        public void WriteColored(SyntaxNodeOrToken nodeOrToken, string color)
        {
            WhiteTriviaList(nodeOrToken.GetLeadingTrivia());
            codeBuilder.Append("<color=");
            codeBuilder.Append(color);
            codeBuilder.Append(">");
            codeBuilder.Append(nodeOrToken.ToString());
            codeBuilder.Append("</color>");
            WhiteTriviaList(nodeOrToken.GetTrailingTrivia());
        }
        
        public void Write(SyntaxNodeOrToken nodeOrToken)
        {
            // if (nodeOrToken.HasLeadingTrivia)
            // {
            //     codeBuilder.Append(nodeOrToken.GetLeadingTrivia());
            // }
            WhiteTriviaList(nodeOrToken.GetLeadingTrivia());
            codeBuilder.Append(nodeOrToken.ToString());
            WhiteTriviaList(nodeOrToken.GetTrailingTrivia());
            // if (nodeOrToken.HasTrailingTrivia)
            // {
            //     codeBuilder.Append(nodeOrToken.GetTrailingTrivia());
            // }
        }

        private void WhiteTriviaList(SyntaxTriviaList triviaList)
        {
            if (!triviaList.Any()) return;
            foreach (var trivia in triviaList)
            {
                var syntaxKind = trivia.Kind();
                if (syntaxKind == SyntaxKind.SingleLineCommentTrivia
                 || syntaxKind == SyntaxKind.MultiLineCommentTrivia
                 || syntaxKind == SyntaxKind.SingleLineDocumentationCommentTrivia
                 || syntaxKind == SyntaxKind.MultiLineDocumentationCommentTrivia)
                {
                    codeBuilder.Append("<color=");
                    codeBuilder.Append(context.Style.CommentColor);
                    codeBuilder.Append(">");
                    codeBuilder.Append(trivia.ToString());
                    codeBuilder.Append("</color>");
                } 
                else
                {
                    codeBuilder.Append(trivia.ToString());
                }
            }
        }

        public void Dispose()
        {
            // if (syntaxNodeOrToken.HasTrailingTrivia) 
            //     codeBuilder.Append(syntaxNodeOrToken.GetTrailingTrivia().ToFullString());
        }
    }
}