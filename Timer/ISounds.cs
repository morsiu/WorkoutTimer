using System;

namespace Timer
{
    internal interface ISounds
    {
        ISound Sound(Frequency frequency, TimeSpan duration);
        
        ISound SeriesOfSound(Frequency frequency, TimeSpan duration, TimeSpan pause, int count);
    }
}