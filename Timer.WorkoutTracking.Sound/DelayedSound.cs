using System;
using System.Threading;
using System.Threading.Tasks;

namespace Timer.WorkoutTracking.Sound
{
    internal sealed class DelayedSound : ISoundEffect
    {
        private readonly TimeSpan _delay;
        private readonly ISound _sound;

        public DelayedSound(TimeSpan delay, ISound sound)
        {
            _delay = delay;
            _sound = sound;
        }

        public async Task Play(CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);
            _sound.PlayAsynchronously();
        }
    }
}