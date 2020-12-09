using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkoutTimer.Tracking.Sound
{
    internal sealed class SoundCountdown : ISoundEffect
    {
        private readonly TimeSpan _duration;
        private readonly ISound _nonLastSound;
        private readonly ISound _lastSound;

        public SoundCountdown(TimeSpan duration, ISound nonLastSound, ISound lastSound)
        {
            _duration = duration;
            _nonLastSound = nonLastSound;
            _lastSound = lastSound;
        }

        public async Task Play(CancellationToken cancellationToken)
        {
            await new SoundsBetweenSteps(Steps(), _nonLastSound).Run(cancellationToken);
            _lastSound.PlayAsynchronously();
        }

        private IEnumerable<TimeSpan> Steps()
        {
            return Enumerable.Repeat(TimeSpan.FromSeconds(1), 5)
                .Scan(TimeSpan.Zero, (x, y) => x + y)
                .TakeWhile(x => x.Sum < _duration)
                .Append(
                    last => (Item: _duration.Subtract(last.Sum), Sum: _duration),
                    () => (Item: _duration, Sum: _duration))
                .Reverse()
                .Select(x => x.Item);
        }
    }
}