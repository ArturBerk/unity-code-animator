using UnityEngine;

namespace CodeAnimator
{
    [ExecuteAlways, ExecuteInEditMode]
    public class LexicalRuleSet
    {
        private ILexicalRule[] rules;
        private string New = "Hello";
        private float f = 0.0f;
        private Token token;

        public void DoSomething<Type1, Type2>()
        {
            Debug.Log("123");
        }

        public LexicalRuleSet(params ILexicalRule[] rules)
        {
            this.rules = rules;
        }

        public bool TryGetToken(string s, int startIndex, out Token token)
        {
            foreach (var lexicalRule in rules)
            {
                if (lexicalRule.TryGetToken(s, startIndex, out token)) return true;
            }

            token = default;

            return false;
        }
    }
}