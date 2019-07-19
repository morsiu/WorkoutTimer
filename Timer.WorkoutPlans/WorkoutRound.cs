using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timer.WorkoutPlans
{
    internal sealed class WorkoutRound
    {
        private readonly ImmutableArray<(WorkoutType Type, Duration Duration)> _workouts;

        public WorkoutRound()
            : this(ImmutableArray<(WorkoutType, Duration)>.Empty)
        {
        }

        private WorkoutRound(ImmutableArray<(WorkoutType, Duration)> workouts)
        {
            _workouts = workouts;
        }

        public WorkoutRound AddBreak(Duration duration) =>
            new WorkoutRound(_workouts.Add((WorkoutType.Break, duration)));

        public WorkoutRound AddExercise(Duration duration) =>
            new WorkoutRound(_workouts.Add((WorkoutType.Exercise, duration)));

        public IEnumerable<T> Enumerate<T>(
            WorkoutPlanVisitor<T> visitor,
            Round round)
        {
            foreach (var step in _workouts)
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

        private enum WorkoutType
        {
            Exercise,
            Break
        }
    }
}