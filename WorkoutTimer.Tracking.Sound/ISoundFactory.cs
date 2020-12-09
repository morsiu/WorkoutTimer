using System;

namespace WorkoutTimer.Tracking.Sound
{
    public interface ISoundFactory
    {
        ISound Sound(Frequency frequency, TimeSpan duration);
        
        ISound SeriesOfSound(Frequency frequency, TimeSpan duration, TimeSpan pause, int count);
    }
}