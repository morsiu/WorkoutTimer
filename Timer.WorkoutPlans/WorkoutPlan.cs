using System;
using System.Collections.Generic;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlan
    {
        private readonly Duration? _warmup;
        private readonly WorkoutRound _round;
        private readonly Count _rounds;

        public WorkoutPlan()
            : this(new WorkoutRound(), new Count(1), default)
        {
        }

        private WorkoutPlan(WorkoutRound round, Count rounds, Duration? warmup)
        {
            _round = round;
            _rounds = rounds;
            _warmup = warmup;
        }

        public (T Warmup, (Count Number, IEnumerable<T> Workouts) Round) Definition<T>(
            Func<Duration?, T> warmup,
            Func<Duration, T> exercise,
            Func<Duration, T> @break)
        {
            return (
                warmup(_warmup), (
                    _rounds,
                    _round.Definition(exercise, @break)));
        }

        public IEnumerable<(Round Round, IEnumerable<T> Workouts)> EnumerateHierarchically<T>(WorkoutPlanVisitor<T> visitor)
        {
            if (!_round.HasExercise)
            {
                yield break;
            }
            foreach (var round in _rounds.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                yield return (round, Round());

                IEnumerable<T> Round()
                {
                    if (round.IsFirst && _warmup is Duration duration && visitor.VisitWarmup(duration, out var warmup))
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
            if (!_round.HasExercise)
            {
                yield break;
            }
            if (_warmup is Duration duration && visitor.VisitWarmup(duration, out var warmup))
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
