using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeAnimator
{
    public class DelayCommand : IAnimatorCommand
    {
        private readonly float delay;

        public DelayCommand(float delay)
        {
            this.delay = delay;
        }

        public UniTask Apply(CodeAnimator animator, CancellationToken cancellationToken)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(delay)).AttachExternalCancellation(cancellationToken);
        }

        public UniTask Apply(CodeAnimator2 animator, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(delay * 0.1f)).AttachExternalCancellation(cancellationToken);
        }
    }
}