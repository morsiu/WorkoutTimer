using System;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlanVisitor<T>
    {
        private readonly Func<Duration, T> _warmup;
        private readonly Func<Round, Index, Duration, T> _break;
        private readonly Func<Round, Index, Duration, T> _exerciseWithDuration;
        private readonly Func<Round, Index, T> _exerciseWithoutDuration;

        public WorkoutPlanVisitor()
        {
        }

        private WorkoutPlanVisitor(
            Func<Duration, T> warmup,
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exerciseWithDuration,
            Func<Round, Index, T> exerciseWithoutDuration)
        {
            _warmup = warmup;
            _break = @break;
            _exerciseWithDuration = exerciseWithDuration;
            _exerciseWithoutDuration = exerciseWithoutDuration;
        }

        public WorkoutPlanVisitor<T> OnWarmup(Func<Duration, T> map) =>
            new(map, _break, _exerciseWithDuration, _exerciseWithoutDuration);

        public WorkoutPlanVisitor<T> OnBreak(Func<Round, Index, Duration, T> map) =>
            new(_warmup, map, _exerciseWithDuration, _exerciseWithoutDuration);

        public WorkoutPlanVisitor<T> OnExercise(Func<Round, Index, T> map) =>
            new(_warmup, _break, _exerciseWithDuration, map);

        public WorkoutPlanVisitor<T> OnExercise(Func<Round, Index, Duration, T> map) =>
            new(_warmup, _break, map, _exerciseWithoutDuration);

        internal bool VisitWarmup(Duration duration, out T result)
        {
            result = _warmup != null
                ? _warmup(duration)
                : default;
            return _warmup != null;
        }

        internal bool VisitExercise(Round round, Index index, out T result)
        {
            result = _exerciseWithoutDuration != null
                ? _exerciseWithoutDuration(round, index)
                : default;
            return _exerciseWithoutDuration != null;
        }

        internal bool VisitExercise(Round round, Index index, Duration duration, out T result)
        {
            result = _exerciseWithDuration != null
                ? _exerciseWithDuration(round, index, duration)
                : default;
            return _exerciseWithDuration != null;
        }

        internal bool VisitBreak(Round round, Index index, Duration duration, out T result)
        {
            result = _break != null
                ? _break(round, index, duration)
                : default;
            return _break != null;
        }
    }
}