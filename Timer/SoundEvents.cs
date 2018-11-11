using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    internal sealed class SoundEvents : IEvents
    {
        private readonly ISounds _sounds;
        private static readonly Frequency PipFrequency = Frequency.FromHertz(1000);

        public SoundEvents(ISounds sounds)
        {
            _sounds = sounds;
        }

        public IEvent WarmUp(TimeSpan duration) => new SoundCountdown(duration, Beep(), Pip());

        public IEvent Exercise(TimeSpan duration) => new PlaySoundAtTheEnd(duration, Pip());

        public IEvent Break(TimeSpan duration) => new SoundCountdown(duration, Beep(), Pip());

        public IEvent SetDone() => new PlaySound(DoublePip());

        public IEvent AllDone() => new PlaySound(LongPip());

        private ISound DoublePip() =>
            _sounds.SeriesOfSound(
                frequency: PipFrequency,
                duration: TimeSpan.FromSeconds(0.2),
                pause: TimeSpan.FromSeconds(0.1),
                count: 2);
        
        private ISound LongPip() => Pip(TimeSpan.FromSeconds(1));

        private ISound Pip() => Pip(TimeSpan.FromSeconds(0.2));
        
        private ISound Pip(TimeSpan duration) => _sounds.Sound(PipFrequency, duration);
        
        private ISound Beep() => _sounds.Sound(Frequency.FromHertz(700), TimeSpan.FromSeconds(0.1));

        private sealed class SoundCountdown : IEvent
        {
            private readonly TimeSpan _duration;
            private readonly ISound _beep;
            private readonly ISound _pip;

            public SoundCountdown(TimeSpan duration, ISound beep, ISound pip)
            {
                _duration = duration;
                _beep = beep;
                _pip = pip;
            }

            public async Task Run(CancellationToken cancellation)
            {
                await new PlaySoundBetweenSteps(Steps(), _beep).Run(cancellation);
                _pip.PlayAsynchronously();
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

        private sealed class PlaySound : IEvent
        {
            private readonly ISound _sound;

            public PlaySound(ISound sound)
            {
                _sound = sound;
            }

            public Task Run(CancellationToken cancellation)
            {
                _sound.PlayAsynchronously();
                return Task.CompletedTask;
            }
        }

        private sealed class PlaySoundAtTheEnd : IEvent
        {
            private readonly TimeSpan _duration;
            private readonly ISound _sound;

            public PlaySoundAtTheEnd(TimeSpan duration, ISound sound)
            {
                _duration = duration;
                _sound = sound;
            }

            public async Task Run(CancellationToken cancellation)
            {
                await Task.Delay(_duration, cancellation);
                _sound.PlayAsynchronously();
            }
        }
    }
}