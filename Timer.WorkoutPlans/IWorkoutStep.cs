using System;

namespace Timer.WorkoutPlans
{
    public interface IWorkoutStep
    {
        T Map<T>(
            Func<Duration, T> exercise,
            Func<Duration, T> @break);
    }
}