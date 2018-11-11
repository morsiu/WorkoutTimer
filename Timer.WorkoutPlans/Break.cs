using System;

namespace Timer.WorkoutPlans
{
    public sealed class Break : IWorkoutStep
    {
        private readonly Duration _duration;

        public Break(Duration duration)
        {
            _duration = duration;
        }

        public T Map<T>(Func<Duration, T> exercise, Func<Duration, T> @break)
        {
            return @break(_duration);
        }
    }
}
