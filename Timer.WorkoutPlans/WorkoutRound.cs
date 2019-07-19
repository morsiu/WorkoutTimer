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

        public WorkoutRound AddBreakWorkout(Duration duration) =>
            new WorkoutRound(_workouts.Add((WorkoutType.Break, duration)));

        public WorkoutRound AddExerciseWorkout(Duration duration) =>
            new WorkoutRound(_workouts.Add((WorkoutType.Exercise, duration)));

        public IEnumerable<T> Select<T>(Func<Duration, T> exercise, Func<Duration, T> @break)
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