using System;

namespace Timer.WorkoutPlans
{
    public interface IWorkoutStep
    {
        T Map<T>(
            Func<Duration, T> warmUp,
            Func<Duration, T> exercise,
            Func<Duration, T> @break,
            Func<T> setDone,
            Func<T> workoutDone);
    }
}