using System;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTimer.Plans;
using Index = WorkoutTimer.Plans.Index;

namespace WorkoutTimer.Tracking
{
    internal sealed class ExerciseWithoutDuration : TrackedWorkout
    {
        private readonly Index _index;
        private readonly Round _round;
        private readonly TaskCompletionSource _complete;

        public ExerciseWithoutDuration(Round round, Index index)
        {
            _round = round;
            _index = index;
            _complete = new();
        }

        public override T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exerciseWithDuration,
            Func<Round, Index, Action, T> exerciseWithoutDuration,
            Func<Duration, T> warmup)
        {
            return exerciseWithoutDuration(_round, _index, Complete);
        }

        public override Task Track(CancellationToken cancellationToken)
        {
            var registration = cancellationToken.Register(() => _complete.TrySetCanceled(cancellationToken));
            _complete.Task.ContinueWith(_ => registration.Dispose(), CancellationToken.None);
            return _complete.Task;
        }

        private void Complete()
        {
            _complete.TrySetResult();
        }
    }
}
