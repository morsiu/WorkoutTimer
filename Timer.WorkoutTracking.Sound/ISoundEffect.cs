using System.Threading;
using System.Threading.Tasks;

namespace Timer.WorkoutTracking.Sound
{
    public interface ISoundEffect
    {
        Task Play(CancellationToken cancellationToken);
    }
}