using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlan
    {
        private readonly Duration _warmup;
        private readonly WorkoutRound _round;
        private readonly Count _rounds;

        public WorkoutPlan()
            : this(new WorkoutRound(), new Count(1), new Duration(0))
        {
        }

        private WorkoutPlan(WorkoutRound round, Count rounds, Duration warmup)
        {
            _round = round;
            _rounds = rounds;
            _warmup = warmup;
        }

        public IEnumerable<(Round Round, IEnumerable<T> Workouts)> EnumerateHierarchically<T>(WorkoutPlanVisitor<T> visitor)
        {
            foreach (var round in _rounds.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                yield return (round, Round());

                IEnumerable<T> Round()
                {
                    if (round.IsFirst && _warmup.TotalSeconds > 0 && visitor.VisitWarmup(_warmup, out var warmup))
                    {
                        yield return warmup;
                    }
                    foreach (var item in _round.Enumerate(visitor, round))
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<T> EnumerateLinearly<T>(WorkoutPlanVisitor<T> visitor)
        {
            if (_warmup.TotalSeconds > 0 && visitor.VisitWarmup(_warmup, out var warmup))
            {
                yield return warmup;
            }
            foreach (var round in _rounds.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                foreach (var item in _round.Enumerate(visitor, round))
                {
                    yield return item;
                }
            }
        }

        public WorkoutPlan AddBreak(Duration duration)
        {
            return new WorkoutPlan(
                _round.AddBreak(duration),
                _rounds,
                _warmup);
        }

        public WorkoutPlan AddExercise(Duration duration)
        {
            return new WorkoutPlan(
                _round.AddExercise(duration),
                _rounds,
                _warmup);
        }

        public IEnumerable<(Round Round, IEnumerable<T> Workouts)> Rounds<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break)
        {
            var visitor =
                new WorkoutPlanVisitor<T>()
                    .OnWarmup(warmUp)
                    .OnExercise(exercise)
                    .OnBreak(@break);
            return EnumerateHierarchically(visitor);
        }

        public IEnumerable<T> Round<T>(
            Func<Duration, T> exercise,
            Func<Duration, T> @break)
        {
            var visitor =
                new WorkoutPlanVisitor<T>()
                    .OnExercise((x, y) => exercise(y))
                    .OnBreak((x, y) => @break(y));
            return EnumerateLinearly(visitor);
        }

        [Pure]
        public IEnumerable<T> Workouts<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break,
            Func<Round, T> nonLastRoundDone,
            Func<Round, T> lastRoundDone)
        {
            var visitor =
                new WorkoutPlanVisitor<T>()
                    .OnWarmup(warmUp)
                    .OnExercise(exercise)
                    .OnBreak(@break)
                    .OnRoundDone(nonLastRoundDone, lastRoundDone);
            return EnumerateLinearly(visitor);
        }

        public WorkoutPlan WithCountdown(Duration value)
        {
            return new WorkoutPlan(
                _round,
                _rounds,
                warmup: value);
        }

        public WorkoutPlan WithRound(Count value)
        {
            return new WorkoutPlan(
                _round,
                rounds: value,
                _warmup);
        }
    }
}
