using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Timer.WorkoutPlans
{
    internal sealed class WorkoutRound
    {
        private readonly ImmutableArray<(WorkoutType Type, Duration Duration)> _workouts;
        private readonly int? _lastExercise;

        public WorkoutRound()
            : this(ImmutableArray<(WorkoutType, Duration)>.Empty, lastExercise: null)
        {
        }

        private WorkoutRound(
            ImmutableArray<(WorkoutType, Duration)> workouts,
            int? lastExercise)
        {
            _workouts = workouts;
            _lastExercise = lastExercise;
        }

        public WorkoutRound AddBreak(Duration duration) =>
            new WorkoutRound(
                _workouts.Add((WorkoutType.Break, duration)), _lastExercise);

        public WorkoutRound AddExercise(Duration duration) =>
            new WorkoutRound(
                _workouts.Add((WorkoutType.Exercise, duration)), lastExercise: _workouts.Length);

        public bool HasExercise => _lastExercise.HasValue;

        public IEnumerable<T> Enumerate<T>(
            WorkoutPlanVisitor<T> visitor,
            Round round)
        {
            if (!(_lastExercise is int lastExercise))
            {
                yield break;
            }
            var workouts =
                _workouts.Take(
                    round.IsLast
                        ? lastExercise + 1
                        : _workouts.Length);
            foreach (var step in workouts)
            {
                switch (step.Type)
                {
                    case WorkoutType.Exercise:
                        if (visitor.VisitExercise(round, step.Duration, out var exercise))
                        {
                            yield return exercise;
                        }
                        break;
                    case WorkoutType.Break:
                        if (visitor.VisitBreak(round, step.Duration, out var @break))
                        {
                            yield return @break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            T roundEnd;
            if (round.IsLast
                ? visitor.VisitLastRound(round, out roundEnd)
                : visitor.VisitNonLastRound(round, out roundEnd))
            {
                yield return roundEnd;
            }
        }


        public IEnumerable<T> Definition<T>(
            Func<Duration, T> exercise,
            Func<Duration, T> @break)
        {
            foreach (var step in _workouts)
            {
                switch (step.Type)
                {
                    case WorkoutType.Exercise:
                        yield return exercise(step.Duration);
                        break;
                    case WorkoutType.Break:
                        yield return @break(step.Duration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private enum WorkoutType
        {
            Exercise,
            Break
        }
    }
}