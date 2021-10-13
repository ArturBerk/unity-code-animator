using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeAnimator
{
    public class LoadFromFileCommand : IAnimatorCommand
    {
        private string filepath;
        private bool isAnimated = true;

        public LoadFromFileCommand(string filepath)
        {
            this.filepath = filepath;
        }

        public LoadFromFileCommand(string filepath, bool isAnimated)
        {
            this.filepath = filepath;
            this.isAnimated = isAnimated;
        }

        public UniTask Apply(CodeAnimator animator, CancellationToken cancellationToken)
        {
            var code = File.ReadAllText(filepath);

            var tree = SyntaxParser.Parse(code);
            code = SyntaxHighlighter.Highlight(tree, Style.VisualStudio);
            //Debug.Log(code);
            
            return animator.SetLinesAsync(code.Split('\n'), filepath);
        }

        public UniTask Apply(CodeAnimator2 animator, CancellationToken cancellationToken = default)
        {
            var code = File.ReadAllText(filepath);
            return animator.Apply(code, isAnimated);
        }
    }
}