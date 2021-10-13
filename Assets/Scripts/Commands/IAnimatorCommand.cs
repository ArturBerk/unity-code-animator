using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeAnimator
{
    public interface IAnimatorCommand
    {
        UniTask Apply(CodeAnimator animator, CancellationToken cancellationToken = default);
        UniTask Apply(CodeAnimator2 animator, CancellationToken cancellationToken = default);
    }
}