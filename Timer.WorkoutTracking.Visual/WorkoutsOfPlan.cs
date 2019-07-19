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
            var workouts = new Workouts();
            _rounds =
                plan.EnumerateHierarchically(
                        new WorkoutPlanVisitor<(Duration Duration, IWorkout Workout)>()
                            .OnWarmup(workouts.Warmup)
                            .OnExercise(workouts.Exercise)
                            .OnBreak(workouts.Break))
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