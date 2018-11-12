using System;

namespace Timer.SoundEffects
{
    public interface ISoundFactory
    {
        ISound Sound(Frequency frequency, TimeSpan duration);
        
        ISound SeriesOfSound(Frequency frequency, TimeSpan duration, TimeSpan pause, int count);
    }
}