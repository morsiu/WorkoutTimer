using System;
using System.Threading;
using System.Threading.Tasks;

namespace Timer.WorkoutTracking.Sound
{
    internal sealed class Delay : ISoundEffect
    {
        private readonly TimeSpan _delay;

        public Delay(TimeSpan delay)
        {
            _delay = delay;
        }

        public Task Play(CancellationToken cancellationToken)
        {
            return Task.Delay(_delay, cancellationToken);
        }
    }
}