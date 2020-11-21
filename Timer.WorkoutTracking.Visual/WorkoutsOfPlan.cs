using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    internal sealed class WorkoutsOfPlan
    {
        private readonly ImmutableDictionary<Round, ImmutableArray<(Duration Duration, IWorkout Workout)>> _rounds;

        public WorkoutsOfPlan(WorkoutPlan plan)
        {
            _rounds =
                plan.EnumerateHierarchically(
                        new WorkoutPlanVisitor<(Duration Duration, IWorkout Workout)>()
                            .OnWarmup(duration => (duration, new Workout(WorkoutType.WarmUp, duration, round: null)))
                            .OnExercise((round, _,  duration) => (duration, new Workout(WorkoutType.Exercise, duration, round)))
                            .OnBreak((round, _, duration) => (duration, new Workout(WorkoutType.Break, duration, round))))
                    .ToImmutableDictionary(x => x.Round, x => x.Workouts.ToImmutableArray());
        }

        public IEnumerable<Round> Rounds() => _rounds.Keys;

        public IEnumerable<(Duration Duration, IWorkout Workout)> OfRound(Round round)
        {
            return _rounds.TryGetValue(round, out var rounds)
                ? rounds
                : Enumerable.Empty<(Duration Duration, IWorkout Workout)>();
        }
    }
}