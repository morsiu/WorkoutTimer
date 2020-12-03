using System;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;
using Index = Timer.WorkoutPlans.Index;

namespace Timer.WorkoutTracking
{
    internal sealed class TrackedWorkout : ITrackedWorkout
    {
        private readonly Round? _round;
        private readonly Index? _index;
        private readonly Duration? _duration;
        private readonly TrackedWorkoutType _type;

        private TrackedWorkout(
            Round? round,
            Index? index,
            Duration? duration,
            TrackedWorkoutType type)
        {
            _round = round;
            _index = index;
            _duration = duration;
            _type = type;
        }

        public static TrackedWorkout Break(Round round, Index index, Duration duration) =>
            new TrackedWorkout(round, index, duration, TrackedWorkoutType.Break);

        public static TrackedWorkout Exercise(Round round, Index index, Duration duration) =>
            new TrackedWorkout(round, index, duration, TrackedWorkoutType.Exercise);

        public static TrackedWorkout Warmup(Duration duration) =>
            new TrackedWorkout(null, null, duration, TrackedWorkoutType.Warmup);

        internal Task Track(CancellationToken cancellationToken) =>
            _duration is { } duration
                ? Task.Delay(duration.ToTimeSpan(), cancellationToken)
                : Task.CompletedTask;

        public T Match<T>(
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exercise,
            Func<Duration, T> warmup)
        {
            return _type switch
            {
                TrackedWorkoutType.Break => @break(_round.Value, _index.Value, _duration.Value),
                TrackedWorkoutType.Exercise => exercise(_round.Value, _index.Value, _duration.Value),
                TrackedWorkoutType.Warmup => warmup(_duration.Value),
                _ => throw new NotImplementedException()
            };
        }
    }
}
