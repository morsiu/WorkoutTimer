using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Tracking.Visual
{
    internal sealed class WorkoutsOfPlan
    {
        private readonly ImmutableSortedDictionary<Round, ImmutableSortedDictionary<Index, IWorkout>> _rounds;

        public WorkoutsOfPlan(WorkoutPlan plan)
        {
            _rounds =
                plan.Enumerate(
                        new WorkoutPlanVisitor<(Index Index, IWorkout Workout)>()
                            .OnWarmup(duration => (
                                new Index(null),
                                new Workout(
                                    WorkoutType.WarmUp,
                                    duration,
                                    index: null,
                                    round: null,
                                    useManualCompletion: false)))
                            .OnExercise((round, index, duration) => (
                                new Index(index),
                                new Workout(
                                    WorkoutType.Exercise,
                                    duration,
                                    index.Number,
                                    round,
                                    useManualCompletion: false)))
                            .OnExercise((round, index) => (
                                new Index(index),
                                new Workout(
                                    WorkoutType.Exercise,
                                    null,
                                    index.Number,
                                    round,
                                    useManualCompletion: true)))
                            .OnBreak((round, index, duration) => (
                                new Index(index),
                                new Workout(
                                    WorkoutType.Break,
                                    duration,
                                    index.Number,
                                    round,
                                    useManualCompletion: false))))
                    .ToImmutableSortedDictionary(
                        x => x.Round,
                        x => x.Workouts.ToImmutableSortedDictionary(x => x.Index, x => x.Workout));
        }

        public IEnumerable<IWorkout> WorkoutsOfRound(Round round)
        {
            return _rounds.TryGetValue(round, out var workouts)
                ? workouts.Values
                : Enumerable.Empty<IWorkout>();
        }

        public IWorkout Workout(ITrackedWorkout workout)
        {
            return workout.Match(
                (round, index, _) => _rounds[round][new Index(index)],
                (round, index, _) => _rounds[round][new Index(index)],
                (round, index, _) => _rounds[round][new Index(index)],
                _ => _rounds.Values.First()[new Index()]);
        }

        private readonly struct Index : IComparable<Index>
        {
            private readonly Plans.Index? _index;

            public Index(Plans.Index? index) => _index = index;

            public int CompareTo(Index other) =>
                Comparer<Plans.Index?>.Default.Compare(_index, other._index);
        }
    }
}