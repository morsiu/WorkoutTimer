using System.Threading;
using System.Threading.Tasks;

namespace Timer.SoundEffects
{
    public interface ISoundEffect
    {
        Task Play(CancellationToken cancellationToken);
    }
}