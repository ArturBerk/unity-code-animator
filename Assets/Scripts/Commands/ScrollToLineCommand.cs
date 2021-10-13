using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeAnimator
{
    public class ScrollToLineCommand : IAnimatorCommand
    {
        private int line;

        public ScrollToLineCommand(int line)
        {
            this.line = line;
        }

        public UniTask Apply(CodeAnimator animator, CancellationToken cancellationToken)
        {
            return animator.Scroll(line);
        }

        public UniTask Apply(CodeAnimator2 animator, CancellationToken cancellationToken = default)
        {
            return animator.Scroll(line);
        }
    }
}