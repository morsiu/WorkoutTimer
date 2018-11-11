using System;

namespace Timer
{
    internal interface IEvents
    {
        IEvent Exercise(TimeSpan duration);

        IEvent Break(TimeSpan duration);
    }
}