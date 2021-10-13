using System.Collections.Generic;
using UnityEngine;

namespace CodeAnimator
{
    public class Colorizer
    {
        private Dictionary<TokenType, IColorizer> formatters = new Dictionary<TokenType, IColorizer>();
        private readonly static string specialWordsColor = new Color(0.3372549f, 0.6117647f, 0.8392157f).ToRichTextFormat();
        private readonly static string usingWordColor = new Color(0.772549f, 0.5254902f, 0.7529412f).ToRichTextFormat();
        private readonly static string defaultWordColor = new Color(0.6117647f, 0.8627451f, 0.9960784f).ToRichTextFormat();
        private readonly static string functionWordColor = new Color(0.8627451f, 0.8627451f, 0.6666667f).ToRichTextFormat();

        public Colorizer()
        {
            formatters.Add(TokenType.String, new SimpleColorizer(Color.white.ToRichTextFormat()));
            formatters.Add(TokenType.Delimiter, new SimpleColorizer(defaultWordColor));
            formatters.Add(TokenType.Identifier, new IdentifierColorizer(new Dictionary<string, string>()
                {
                    {"public", specialWordsColor},
                    {"protected", specialWordsColor},
                    {"private", specialWordsColor},
                    {"internal", specialWordsColor},
                    
                    {"namespace", specialWordsColor},
                    {"class", specialWordsColor},
                    {"struct", specialWordsColor},
                    {"enum", specialWordsColor},
                    
                    {"int", specialWordsColor},
                    {"uint", specialWordsColor},
                    {"float", specialWordsColor},
                    {"double", specialWordsColor},
                    {"short", specialWordsColor},
                    {"ushort", specialWordsColor},
                    {"byte", specialWordsColor},
                    {"bool", specialWordsColor},
                    {"string", specialWordsColor},
                    {"decimal", specialWordsColor},
                    
                    {"var", specialWordsColor},
                    {"override", specialWordsColor},
                    {"abstract", specialWordsColor},
                    {"static", specialWordsColor},
                    {"unsafe", specialWordsColor},
                    {"void", specialWordsColor},
                    {"where", specialWordsColor},
                    {"unmanaged", specialWordsColor},
                    {"in", specialWordsColor},
                    {"out", specialWordsColor},
                    {"ref", specialWordsColor},
                    {"false", specialWordsColor},
                    {"true", specialWordsColor},
                    {"typeof", specialWordsColor},
                    {"new", specialWordsColor},
                    {"try", specialWordsColor},
                    {"finally", specialWordsColor},
                    {"catch", specialWordsColor},
                    {"for", specialWordsColor},
                    {"foreach", specialWordsColor},
                    {"is", specialWordsColor},
                    {"as", specialWordsColor},
                    {"if", specialWordsColor},
                    {"else", specialWordsColor},
                    {"get", specialWordsColor},
                    {"set", specialWordsColor},
                    {"return", specialWordsColor},
                    {"default", specialWordsColor},
                    
                    {"using", usingWordColor},
                }, new SimpleColorizer(functionWordColor))
            );
        }

        public string Colorize(string s, Token token)
        {
            var tokenValue = token.ReadTokenValue(s);
            if (!formatters.TryGetValue(token.Type, out var colorizer)) return tokenValue;
            return colorizer.Colorize(tokenValue);
        }
    }

    public interface IColorizer
    {
        string Colorize(string token);
    }

    public abstract class ChainedColorizer : IColorizer
    {
        private IColorizer nextColorizer;

        protected ChainedColorizer(IColorizer nextColorizer)
        {
            this.nextColorizer = nextColorizer;
        }

        public virtual string Colorize(string token)
        {
            return nextColorizer?.Colorize(token) ?? token;
        }
    }

    public class SimpleColorizer : IColorizer
    {
        private string colorFormat;

        public SimpleColorizer(string colorFormat)
        {
            this.colorFormat = colorFormat;
        }

        public string Colorize(string token)
        {
            return string.Format(colorFormat, token);
        }
    }

    public class IdentifierColorizer : ChainedColorizer
    {
        private Dictionary<string, string> formatters;

        public IdentifierColorizer(Dictionary<string, string> formatters, IColorizer colorizer = null) : base(colorizer)
        {
            this.formatters = formatters;
        }

        public override string Colorize(string token)
        {
            if (!formatters.TryGetValue(token, out var formatter)) return base.Colorize(token);
            return string.Format(formatter, token);
        }
    }

    public static class ColorizerExtensions
    {
        public static string ToRichTextFormat(this Color color)
        {
            return $"<color={color.ToHexString()}>{{0}}</color>";
        }

        public static string ToHexString(this Color color)
        {
            return'#'+
                  ((byte) (color.r * 255)).ToString("X2") +
                  ((byte) (color.g * 255)).ToString("X2") +
                  ((byte) (color.b * 255)).ToString("X2");
        }

        public static string ToAlphaHexString(this Color color)
        {
            return '#'+
                ((byte) (color.r * 255)).ToString("X2") +
                ((byte) (color.g * 255)).ToString("X2") +
                ((byte) (color.b * 255)).ToString("X2") +
                ((byte) (color.a * 255)).ToString("X2");
        }
    }
}