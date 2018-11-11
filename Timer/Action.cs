using System;
using Timer.ExercisePlans;

namespace Timer
{
    internal sealed class Action : IAction
    {
        public TimeSpan Duration { get; set; }
        
        public ActionPurpose Purpose { get; set; }

        public T Map<T>(
            Func<TimeSpan, T> exercise,
            Func<TimeSpan, T> @break,
            Func<T> invalid)
        {
            if (Duration < TimeSpan.Zero) return invalid();
            switch (Purpose)
            {
                case ActionPurpose.Exercise:
                    return exercise(Duration);
                case ActionPurpose.Break:
                    return @break(Duration);
                default:
                    return invalid();
            }
        }
    }
}