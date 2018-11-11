using System;

namespace Timer.WorkoutPlans
{
    internal sealed class WorkoutDone : IWorkoutStep
    {
        public T Map<T>(Func<Duration, T> warmUp, Func<Duration, T> exercise, Func<Duration, T> @break, Func<T> setDone, Func<T> workoutDone)
        {
            return workoutDone();
        }
    }
}