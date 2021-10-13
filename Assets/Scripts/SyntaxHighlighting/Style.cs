using UnityEngine;

namespace CodeAnimator
{
    public class Style
    {
        public static readonly Style VisualStudio = new Style(
            "#569CD6", 
            "#4EC9B0",
            "#B5CEA8",
            "#D69D85",
            "#D8DB9D",
            "#52A335"
        );
        
        public readonly string KeywordsColor;
        public readonly string StringLiteralColor;
        public readonly string InvocationColor;
        public readonly string LiteralColor;
        public readonly string ModifierColor;
        public readonly string IdentifierColor;
        public readonly string CommentColor;

        public Style(Color keywordsColor, Color identifierColor, Color literalColor, Color stringLiteralColor, Color invocationColor, Color commentColor)
        {
            KeywordsColor = keywordsColor.ToHexString();
            ModifierColor = keywordsColor.ToHexString();
            IdentifierColor = identifierColor.ToHexString();
            LiteralColor = literalColor.ToHexString();
            StringLiteralColor = stringLiteralColor.ToHexString();
            InvocationColor = invocationColor.ToHexString();
            CommentColor = commentColor.ToHexString();
            
        }
        
        public Style(string keywordsColor, string identifierColor, string literalColor, string stringLiteralColor, string invocationColor, string commentColor)
        {
            KeywordsColor = keywordsColor;
            ModifierColor = keywordsColor;
            IdentifierColor = identifierColor;
            LiteralColor = literalColor;
            StringLiteralColor = stringLiteralColor;
            InvocationColor = invocationColor;
            CommentColor = commentColor;
        }
    }
}