using System;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTimer.Plans;
using Index = WorkoutTimer.Plans.Index;

namespace WorkoutTimer.Tracking
{
    internal sealed class Warmup : TrackedWorkout
    {
        private readonly Duration _duration;

        public Warmup(Duration duration)
        {
            _duration = duration;
        }

        public override T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exerciseWithDuration,
            Func<Round, Index, Action, T> exerciseWithoutDuration,
            Func<Duration, T> warmup)
        {
            return warmup(_duration);
        }

        public override Task Track(CancellationToken cancellationToken)
        {
            return Task.Delay(_duration.ToTimeSpan(), cancellationToken);
        }
    }
}
