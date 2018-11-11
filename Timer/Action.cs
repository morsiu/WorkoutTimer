using System;

namespace Timer
{
    internal sealed class Action
    {
        public TimeSpan Duration { get; set; }
        
        public ActionPurpose Purpose { get; set; }

        public IEvent Event(IEvents events)
        {
            if (Duration < TimeSpan.Zero) return new None();
            switch (Purpose)
            {
                case ActionPurpose.Exercise:
                    return events.Exercise(Duration);
                case ActionPurpose.Break:
                    return events.Break(Duration);
                default:
                    return new Delay(Duration);
            }
        }
    }
}