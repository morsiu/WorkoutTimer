using System;

namespace Timer.WorkoutPlans
{
    public sealed class Exercise : IWorkoutStep
    {
        private readonly Duration _duration;

        public Exercise(Duration duration)
        {
            _duration = duration;
        }

        public T Map<T>(Func<Duration, T> exercise, Func<Duration, T> @break)
        {
            return exercise(_duration);
        }
    }
}
