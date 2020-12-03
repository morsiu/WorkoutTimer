using System;
using Timer.WorkoutPlans;
using Index = Timer.WorkoutPlans.Index;

namespace Timer.WorkoutTracking
{
    public interface ITrackedWorkout
    {
        T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exercise,
            Func<Duration, T> warmup);
    }
}
