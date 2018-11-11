using System;

namespace Timer.WorkoutPlans
{
    internal sealed class Exercise : IWorkoutStep
    {
        private readonly Duration _duration;

        public Exercise(Duration duration)
        {
            _duration = duration;
        }

        public T Map<T>(Func<Duration, T> warmUp, Func<Duration, T> exercise, Func<Duration, T> @break, Func<T> setDone, Func<T> workoutDone)
        {
            return exercise(_duration);
        }
    }
}
