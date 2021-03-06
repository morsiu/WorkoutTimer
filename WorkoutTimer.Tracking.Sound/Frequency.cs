namespace WorkoutTimer.Tracking.Sound
{
    public readonly struct Frequency
    {
        private readonly int _hertz;

        public Frequency(int hertz) => _hertz = hertz;

        public static Frequency FromHertz(int hertz) => new(hertz);
        
        public static implicit operator double(Frequency frequency) => frequency._hertz;
    }
}