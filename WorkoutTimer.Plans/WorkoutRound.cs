using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WorkoutTimer.Plans
{
    internal sealed class WorkoutRound
    {
        private readonly ImmutableArray<(WorkoutType Type, Duration? Duration)> _workouts;
        private readonly int? _indexOfLastExercise;

        public WorkoutRound()
            : this(ImmutableArray<(WorkoutType, Duration?)>.Empty, lastExercise: null)
        {
        }

        private WorkoutRound(
            ImmutableArray<(WorkoutType, Duration?)> workouts,
            int? lastExercise)
        {
            _workouts = workouts;
            _indexOfLastExercise = lastExercise;
        }

        public WorkoutRound AddBreak(Duration duration) =>
            new(_workouts.Add((WorkoutType.Break, duration)), _indexOfLastExercise);

        public WorkoutRound AddExercise() =>
            new(_workouts.Add((WorkoutType.Exercise, null)), lastExercise: _workouts.Length);

        public WorkoutRound AddExercise(Duration duration) =>
            new(_workouts.Add((WorkoutType.Exercise, duration)), lastExercise: _workouts.Length);

        public bool HasExercise => _indexOfLastExercise.HasValue;

        public IEnumerable<T> Enumerate<T>(
            WorkoutPlanVisitor<T> visitor,
            Round round)
        {
            if (!(_indexOfLastExercise is { } indexOfLastExercise))
            {
                yield break;
            }
            var workouts =
                _workouts.Take(
                    round.IsLast
                        ? indexOfLastExercise + 1
                        : _workouts.Length)
                    .Select((x, i) => (x, new Index(i)));
            foreach (var (workout, index) in workouts)
            {
                switch (workout.Type, workout.Duration)
                {
                    case (WorkoutType.Exercise, { } duration):
                        if (visitor.VisitExercise(round, index, duration, out var exerciseWithDuration))
                        {
                            yield return exerciseWithDuration;
                        }
                        break;
                    case (WorkoutType.Exercise, null):
                        if (visitor.VisitExercise(round, index, out var exerciseWithoutDuration))
                        {
                            yield return exerciseWithoutDuration;
                        }
                        break;
                    case (WorkoutType.Break, { } duration):
                        if (visitor.VisitBreak(round, index, duration, out var @break))
                        {
                            yield return @break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        public IEnumerable<T> Definition<T>(
            Func<Duration, T> exerciseWithDuration,
            Func<T> exerciseWithoutDuration,
            Func<Duration, T> @break)
        {
            foreach (var step in _workouts)
            {
                switch (step.Type, step.Duration)
                {
                    case (WorkoutType.Exercise, { } duration):
                        yield return exerciseWithDuration(duration);
                        break;
                    case (WorkoutType.Exercise, null):
                        yield return exerciseWithoutDuration();
                        break;
                    case (WorkoutType.Break, { } duration):
                        yield return @break(duration);
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