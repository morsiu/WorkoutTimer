using System;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Tracking
{
    internal abstract class TrackedWorkout : ITrackedWorkout
    {
        public abstract T Match<T>(
            Func<Round, Plans.Index, Duration, T> @break,
            Func<Round, Plans.Index, Duration, T> exerciseWithDuration,
            Func<Round, Plans.Index, Action, T> exerciseWithoutDuration,
            Func<Duration, T> warmup);

        public abstract Task Track(CancellationToken cancellationToken);
    }
}
