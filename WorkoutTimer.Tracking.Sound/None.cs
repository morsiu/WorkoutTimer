using System.Threading;
using System.Threading.Tasks;

namespace WorkoutTimer.Tracking.Sound
{
    internal sealed class None : ISoundEffect
    {
        public Task Play(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}