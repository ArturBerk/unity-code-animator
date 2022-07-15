using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeAnimator
{
    public static class SimpleHighlighter
    {
        private static Dictionary<string, Func<Style, string>> wordColorizers;

        static SimpleHighlighter()
        {
            wordColorizers = new Dictionary<string, Func<Style, string>>
            {
                { "Shader", context => context.KeywordsColor },
                { "SubShader", context => context.KeywordsColor },
                { "Properties", context => context.KeywordsColor },
                { "Tags", context => context.KeywordsColor },
                { "Pass", context => context.KeywordsColor },
                { "HLSLPROGRAM", context => context.IdentifierColor },
                { "ENDHLSL", context => context.IdentifierColor },
                { "return", context => context.KeywordsColor },
                { "for", context => context.KeywordsColor },
                { "triangle", context => context.KeywordsColor },
                { "inout", context => context.KeywordsColor },
                { "in", context => context.KeywordsColor },
                { "out", context => context.KeywordsColor },
                { "struct", context => context.KeywordsColor },
                { "point", context => context.KeywordsColor },
                { "uniform", context => context.KeywordsColor },
                { "float", context => context.KeywordsColor },
                { "float2", context => context.KeywordsColor },
                { "int", context => context.KeywordsColor },
                { "float3", context => context.KeywordsColor },
                { "float4", context => context.KeywordsColor },
                { "fixed", context => context.KeywordsColor },
                { "fixed2", context => context.KeywordsColor },
                { "fixed3", context => context.KeywordsColor },
                { "fixed4", context => context.KeywordsColor },
                { "float4x4", context => context.KeywordsColor },
                { "sampler2D", context => context.KeywordsColor },
                { "ENDCG", context => context.KeywordsColor },
                { "CGPROGRAM", context => context.KeywordsColor },
                //{ "SV_Target", context => context.ModifierColor },
            };
        }

        private static Regex wordRegex = new Regex("[#\\w\\d]+", RegexOptions.Compiled); 
        private static Regex stringRegex = new Regex("\"[^\"]*\"", RegexOptions.Compiled); 
        
        public static string Highlight(string code, Style style)
        {
            code = HandleMatches(code, wordRegex, (stringBuilder, matchValue) =>
            {
                if (wordColorizers.TryGetValue(matchValue, out var getter))
                {
                    WriteColored(stringBuilder, matchValue, getter(style));
                }
                else if (matchValue.StartsWith("#"))
                {
                    WriteColored(stringBuilder, matchValue, style.KeywordsColor);
                }
                else
                {
                    stringBuilder.Append(matchValue);
                }
            });
            return HandleMatches(code, stringRegex, (builder, s) =>
            {
                WriteColored(builder, s, style.StringLiteralColor);
            });
        }

        private static string HandleMatches(string code, Regex regex, Action<StringBuilder, string> matchHandler)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var wordMatches = regex.Matches(code);
            var currentIndex = 0;
            foreach (Match match in wordMatches)
            {
                var previousSegmentLength = match.Index - currentIndex;
                if (previousSegmentLength > 0)
                    stringBuilder.Append(code.Substring(currentIndex, previousSegmentLength));

                matchHandler(stringBuilder, match.Value);
                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < code.Length)
                stringBuilder.Append(code.Substring(currentIndex, code.Length - currentIndex));

            return stringBuilder.ToString();
        }
        
        private static void WriteColored(StringBuilder codeBuilder, string code, string color)
        {
            codeBuilder.Append("<color=");
            codeBuilder.Append(color);
            codeBuilder.Append(">");
            codeBuilder.Append(code);
            codeBuilder.Append("</color>");
        }
    }
}