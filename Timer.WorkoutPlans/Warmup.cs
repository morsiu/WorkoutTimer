using System;

namespace Timer.WorkoutPlans
{
    internal sealed class Warmup : IWorkoutStep
    {
        public T Map<T>(Func<Duration, T> warmUp, Func<Duration, T> exercise, Func<Duration, T> @break, Func<T> setDone, Func<T> workoutDone)
        {
            return warmUp(Duration.FromSeconds(15));
        }
    }
}