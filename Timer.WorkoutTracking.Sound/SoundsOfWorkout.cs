using System;

namespace Timer.WorkoutTracking.Sound
{
    internal sealed class SoundsOfWorkout
    {
        private readonly ISoundFactory _soundFactory;
        private static readonly Frequency PipFrequency = Frequency.FromHertz(1000);

        public SoundsOfWorkout(ISoundFactory soundFactory)
        {
            _soundFactory = soundFactory;
        }

        public ISoundEffect WarmUp(TimeSpan duration) => new SoundCountdown(duration, Beep(), new Silence());

        public ISoundEffect Exercise() => new Sound(ShortPip());

        public ISoundEffect Exercise(TimeSpan duration) => new Sound(ShortPip()).Then(new Delay(duration));

        public ISoundEffect Break(TimeSpan duration) => new Sound(TwoShortPips()).Then(new SoundCountdown(duration, Beep(), new Silence()));

        public ISoundEffect RoundDone() => new None();

        public ISoundEffect WorkoutDone() => new SingleSound(LongPip());

        private ISound LongPip() => Pip(TimeSpan.FromSeconds(1));

        private ISound ShortPip() => Pip(TimeSpan.FromSeconds(0.2));

        private ISound TwoShortPips() =>
            _soundFactory.SeriesOfSound(
                frequency: PipFrequency,
                duration: TimeSpan.FromSeconds(0.05),
                pause: TimeSpan.FromSeconds(0.1),
                count: 2);
        
        private ISound Pip(TimeSpan duration) => _soundFactory.Sound(PipFrequency, duration);
        
        private ISound Beep() => _soundFactory.Sound(Frequency.FromHertz(700), TimeSpan.FromSeconds(0.1));
    }
}