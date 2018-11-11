using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutRound
    {
        private readonly ImmutableArray<(StepType Type, Duration Duration)> _steps;

        public WorkoutRound()
            : this(ImmutableArray<(StepType, Duration)>.Empty)
        {
        }

        private WorkoutRound(ImmutableArray<(StepType, Duration)> steps)
        {
            _steps = steps;
        }

        public WorkoutRound AddBreak(Duration duration) =>
            new WorkoutRound(_steps.Add((StepType.Break, duration)));

        public WorkoutRound AddExercise(Duration duration) =>
            new WorkoutRound(_steps.Add((StepType.Exercise, duration)));

        internal IEnumerable<T> Select<T>(Func<Duration, T> exercise, Func<Duration, T> @break)
        {
            foreach (var step in _steps)
            {
                switch (step.Type)
                {
                    case StepType.Exercise:
                        yield return exercise(step.Duration);
                        break;
                    case StepType.Break:
                        yield return @break(step.Duration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private enum StepType
        {
            Exercise,
            Break
        }
    }
}