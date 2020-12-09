using System;
using WorkoutTimer.Plans;
using Index = WorkoutTimer.Plans.Index;

namespace WorkoutTimer.Tracking
{
    public interface ITrackedWorkout
    {
        T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exerciseWithDuration,
            Func<Round, Index, Action, T> exerciseWithoutDuration,
            Func<Duration, T> warmup);
    }
}
