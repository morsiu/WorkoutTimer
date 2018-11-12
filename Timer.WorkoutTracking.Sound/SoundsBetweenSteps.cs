using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Timer.SoundEffects
{
    internal sealed class SoundsBetweenSteps
    {
        private readonly IEnumerable<TimeSpan> _steps;
        private readonly ISound _sound;

        public SoundsBetweenSteps(IEnumerable<TimeSpan> steps, ISound sound)
        {
            _steps = steps;
            _sound = sound;
        }

        public async Task Run(CancellationToken cancellation)
        {
            using (var enumerator = _steps.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return;
                var current = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    await Task.Delay(current, cancellation);
                    return;
                }
                var next = enumerator.Current;
                while (true)
                {
                    await Task.Delay(current, cancellation);
                    _sound.PlayAsynchronously();
                    if (!enumerator.MoveNext()) break;
                    current = next;
                    next = enumerator.Current;
                }
                await Task.Delay(next, cancellation);
            }
        }
    }
}