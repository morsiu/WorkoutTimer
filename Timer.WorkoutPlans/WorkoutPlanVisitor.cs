using System;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlanVisitor<T>
    {
        private readonly Func<Duration, T> _warmup;
        private readonly Func<Round, Index, Duration, T> _break;
        private readonly Func<Round, Index, Duration, T> _exercise;

        public WorkoutPlanVisitor()
        {
        }

        private WorkoutPlanVisitor(
            Func<Duration, T> warmup,
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exercise)
        {
            _warmup = warmup;
            _break = @break;
            _exercise = exercise;
        }

        public WorkoutPlanVisitor<T> OnWarmup(Func<Duration, T> map) =>
            new WorkoutPlanVisitor<T>(map, _break, _exercise);

        public WorkoutPlanVisitor<T> OnBreak(Func<Round, Index, Duration, T> map) =>
            new WorkoutPlanVisitor<T>(_warmup, map, _exercise);

        public WorkoutPlanVisitor<T> OnExercise(Func<Round, Index, Duration, T> map) =>
            new WorkoutPlanVisitor<T>(_warmup, _break, map);

        internal bool VisitWarmup(Duration duration, out T result)
        {
            result = _warmup != null
                ? _warmup(duration)
                : default;
            return _warmup != null;
        }

        internal bool VisitExercise(Round round, Index index, Duration duration, out T result)
        {
            result = _exercise != null
                ? _exercise(round, index, duration)
                : default;
            return _exercise != null;
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