using System;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTimer.Plans;
using Index = WorkoutTimer.Plans.Index;

namespace WorkoutTimer.Tracking
{
    internal sealed class Break : TrackedWorkout
    {
        private readonly Duration _duration;
        private readonly Index _index;
        private readonly Round _round;

        public Break(Round round, Index index, Duration duration)
        {
            _duration = duration;
            _round = round;
            _index = index;
        }

        public override T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exerciseWithDuration,
            Func<Round, Index, Action, T> exerciseWithoutDuration,
            Func<Duration, T> warmup)
        {
            return @break(_round, _index, _duration);
        }

        public override Task Track(CancellationToken cancellationToken)
        {
            return Task.Delay(_duration.ToTimeSpan(), cancellationToken);
        }
    }
}
