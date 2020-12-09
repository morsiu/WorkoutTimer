using System.Threading;
using System.Threading.Tasks;

namespace WorkoutTimer.Tracking.Sound
{
    public interface ISoundEffect
    {
        Task Play(CancellationToken cancellationToken);
    }
}