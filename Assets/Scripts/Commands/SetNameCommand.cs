using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeAnimator
{
    public class SetNameCommand: IAnimatorCommand
    {
        private readonly string name;

        public SetNameCommand(string name)
        {
            this.name = name;
        }

        public UniTask Apply(CodeAnimator animator, CancellationToken cancellationToken = default)
        {
            animator.SetName(name);
            return UniTask.CompletedTask;
        }

        public UniTask Apply(CodeAnimator2 animator, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }
    }
}