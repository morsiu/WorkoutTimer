using System;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlanVisitor<T>
    {
        private readonly Func<Duration, T> _warmup;
        private readonly Func<Round, Index, Duration, T> _break;
        private readonly Func<Round, Index, Duration, T> _exercise;
        private readonly Func<Round, T> _nonLastRound;
        private readonly Func<Round, T> _lastRound;

        public WorkoutPlanVisitor()
        {
        }

        private WorkoutPlanVisitor(
            Func<Duration, T> warmup,
            Func<Round, Index, Duration, T> @break,
            Func<Round, Index, Duration, T> exercise,
            Func<Round, T> nonLastRound,
            Func<Round, T> lastRound)
        {
            _warmup = warmup;
            _break = @break;
            _exercise = exercise;
            _nonLastRound = nonLastRound;
            _lastRound = lastRound;
        }

        public WorkoutPlanVisitor<T> OnWarmup(Func<Duration, T> map) =>
            new WorkoutPlanVisitor<T>(map, _break, _exercise, _nonLastRound, _lastRound);

        public WorkoutPlanVisitor<T> OnBreak(Func<Round, Index, Duration, T> map) =>
            new WorkoutPlanVisitor<T>(_warmup, map, _exercise, _nonLastRound, _lastRound);

        public WorkoutPlanVisitor<T> OnExercise(Func<Round, Index, Duration, T> map) =>
            new WorkoutPlanVisitor<T>(_warmup, _break, map, _nonLastRound, _lastRound);

        public WorkoutPlanVisitor<T> OnRoundDone(Func<Round, T> nonLast, Func<Round, T> last) =>
            new WorkoutPlanVisitor<T>(_warmup, _break, _exercise, nonLast, last);

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

        internal bool VisitNonLastRound(Round round, out T result)
        {
            result = _nonLastRound != null
                ? _nonLastRound(round)
                : default;
            return _nonLastRound != null;
        }

        internal bool VisitLastRound(Round round, out T result)
        {
            result = _lastRound != null
                ? _lastRound(round)
                : default;
            return _lastRound != null;
        }
    }
}