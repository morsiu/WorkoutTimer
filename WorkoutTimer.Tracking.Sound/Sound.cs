using System.Threading;
using System.Threading.Tasks;

namespace WorkoutTimer.Tracking.Sound
{
    internal sealed class Sound : ISoundEffect
    {
        private readonly ISound _sound;

        public Sound(ISound sound)
        {
            _sound = sound;
        }

        public Task Play(CancellationToken cancellationToken)
        {
            _sound.PlayAsynchronously();
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            return Task.CompletedTask;
        }
    }
}