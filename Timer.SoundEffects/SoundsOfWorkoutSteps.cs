using System;

namespace Timer.SoundEffects
{
    internal sealed class SoundsOfWorkoutSteps
    {
        private readonly ISoundFactory _soundFactory;
        private static readonly Frequency PipFrequency = Frequency.FromHertz(1000);

        public SoundsOfWorkoutSteps(ISoundFactory soundFactory)
        {
            _soundFactory = soundFactory;
        }

        public ISoundEffect WarmUp(TimeSpan duration) => new SoundCountdown(duration, Beep(), Pip());

        public ISoundEffect Exercise(TimeSpan duration) => new DelayedSound(duration, Pip());

        public ISoundEffect Break(TimeSpan duration) => new SoundCountdown(duration, Beep(), Pip());

        public ISoundEffect SetDone() => new SingleSound(DoublePip());

        public ISoundEffect AllDone() => new SingleSound(LongPip());

        private ISound DoublePip() =>
            _soundFactory.SeriesOfSound(
                frequency: PipFrequency,
                duration: TimeSpan.FromSeconds(0.2),
                pause: TimeSpan.FromSeconds(0.1),
                count: 2);
        
        private ISound LongPip() => Pip(TimeSpan.FromSeconds(1));

        private ISound Pip() => Pip(TimeSpan.FromSeconds(0.2));
        
        private ISound Pip(TimeSpan duration) => _soundFactory.Sound(PipFrequency, duration);
        
        private ISound Beep() => _soundFactory.Sound(Frequency.FromHertz(700), TimeSpan.FromSeconds(0.1));
    }
}