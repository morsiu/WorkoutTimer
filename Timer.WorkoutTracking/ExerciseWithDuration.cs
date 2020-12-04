using System;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;
using Index = Timer.WorkoutPlans.Index;

namespace Timer.WorkoutTracking
{
    internal sealed class ExerciseWithDuration : TrackedWorkout
    {
        private readonly Duration _duration;
        private readonly Index _index;
        private readonly Round _round;

        public ExerciseWithDuration(Round round, Index index, Duration duration)
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
            return exerciseWithDuration(_round, _index, _duration);
        }

        public override Task Track(CancellationToken cancellationToken)
        {
            return Task.Delay(_duration.ToTimeSpan(), cancellationToken);
        }
    }
}
