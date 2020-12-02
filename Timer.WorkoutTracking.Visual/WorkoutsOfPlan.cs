using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    internal sealed class WorkoutsOfPlan
    {
        private readonly ImmutableSortedDictionary<Round, ImmutableSortedDictionary<Index, IWorkout>> _rounds;

        public WorkoutsOfPlan(WorkoutPlan plan)
        {
            _rounds =
                plan.EnumerateHierarchically(
                        new WorkoutPlanVisitor<(Index Index, IWorkout Workout)>()
                            .OnWarmup(duration => (new Index(null), new Workout(WorkoutType.WarmUp, duration, round: null)))
                            .OnExercise((round, index,  duration) => (new Index(index), new Workout(WorkoutType.Exercise, duration, round)))
                            .OnBreak((round, index, duration) => (new Index(index), new Workout(WorkoutType.Break, duration, round))))
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
                _ => _rounds.Values.First()[new Index()]);
        }

        public IEnumerable<Round> Rounds() => _rounds.Keys;

        private readonly struct Index : IComparable<Index>
        {
            private readonly WorkoutPlans.Index? _index;

            public Index(WorkoutPlans.Index? index) => _index = index;

            public int CompareTo(Index other) =>
                Comparer<WorkoutPlans.Index?>.Default.Compare(_index, other._index);
        }
    }
}