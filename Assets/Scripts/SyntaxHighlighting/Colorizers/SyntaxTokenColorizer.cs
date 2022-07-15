using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using UnityEngine;

namespace CodeAnimator
{
    class SyntaxTokenColorizer : INodeHandler<ColorizingContext>
    {
        private static Dictionary<string, Func<ColorizingContext, string>> tokenColorGetters;

        static SyntaxTokenColorizer()
        {
            var keywordGetter = (Func<ColorizingContext, string>)(context => context.Style.KeywordsColor);
            tokenColorGetters = new Dictionary<string, Func<ColorizingContext, string>>
            {
                {"new", keywordGetter},
                {"const", keywordGetter},
                {"for", keywordGetter},
                {"foreach", keywordGetter},
                {"in", keywordGetter},
                {"if", keywordGetter},
                {"else", keywordGetter},
                {"ref", keywordGetter},
                {"out", keywordGetter},
                {"return", keywordGetter},
                {"var", keywordGetter},
                {"try", keywordGetter},
                {"this", keywordGetter},
                {"finally", keywordGetter},
                {"public", keywordGetter},
                {"internal", keywordGetter},
                {"abstract", keywordGetter},
                {"override", keywordGetter},
                {"private", keywordGetter},
                {"protected", keywordGetter},
                {"typeof", keywordGetter},
                {"using", keywordGetter},
                {"where", keywordGetter},
                {"struct", keywordGetter},
                {"class", keywordGetter},
                {"enum", keywordGetter},
                {"interface", keywordGetter},
                {"static", keywordGetter},
                {"unmanaged", keywordGetter},
                {"readonly", keywordGetter},
                {"implicit", keywordGetter},
                {"explicit", keywordGetter},
                {"unchecked", keywordGetter},
                {"switch", keywordGetter},
                {"case", keywordGetter},
                {"break", keywordGetter},
                {"yield", keywordGetter},
                {"virtual", keywordGetter},
                {"operator", keywordGetter},
                {"fixed", keywordGetter},
                {"unsafe", keywordGetter},
                {"while", keywordGetter},
                {"do", keywordGetter},
                {"async", keywordGetter},
                {"await", keywordGetter},
                {"base", keywordGetter},
                {"get", keywordGetter},
                {"set", keywordGetter},
                {"add", keywordGetter},
                {"remove", keywordGetter},
                {"throw", keywordGetter},
                {"is", keywordGetter},
                {"as", keywordGetter},
                {"catch", keywordGetter},
                {"event", keywordGetter},
                {"delegate", keywordGetter},
                {"continue", keywordGetter},
            };
        }
        
        public void Handle(SyntaxNodeOrToken node, INodeVisitor nodeVisitor, ColorizingContext context)
        {
            var token = node.AsToken();
            var text = token.Text;
            using var writer = new NodeWriter(node, context.StringBuilder, context);
            if (tokenColorGetters.TryGetValue(text, out var getter))
            {
                writer.WriteColored(token, getter(context));
            }
            else if (text.StartsWith("#"))
            {
                writer.WriteColored(token, context.Style.KeywordsColor);
            }
            else
            {
                writer.Write(token);
            }
        }
    }
}