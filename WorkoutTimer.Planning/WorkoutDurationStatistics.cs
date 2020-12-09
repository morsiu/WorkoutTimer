using System;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Planning
{
    internal sealed class WorkoutDurationStatistics
    {
        private readonly WorkoutPlan _workoutPlan;

        public WorkoutDurationStatistics(WorkoutPlan workoutPlan)
        {
            _workoutPlan = workoutPlan;
        }

        public TimeSpan Total()
        {
            var round =
                _workoutPlan.Definition(
                        x => x?.ToTimeSpan() ?? TimeSpan.Zero,
                        x => x.ToTimeSpan(),
                        () => TimeSpan.Zero,
                        x => x.ToTimeSpan())
                    .Round;
            var roundDuration = round.Workouts.Aggregate(TimeSpan.Zero, (a, b) => a + b);
            var lastBreakOfRound = LastBreakOfRound();
            return TimeSpan.FromSeconds(roundDuration.TotalSeconds * round.Number) - lastBreakOfRound;
        }

        public TimeSpan ExercisePerRound()
        {
            var workoutsOfRound =
                _workoutPlan.Definition(
                        x => (Duration: x?.ToTimeSpan() ?? TimeSpan.Zero, IsBreak: false),
                        x => (Duration: x.ToTimeSpan(), IsBreak: false),
                        () => (Duration: TimeSpan.Zero, IsBreak: false),
                        x => (Duration: x.ToTimeSpan(), IsBreak: true))
                    .Round.Workouts;
            return workoutsOfRound.Aggregate(
                TimeSpan.Zero,
                (a, b) => b.IsBreak ? a : a.Add(b.Duration));
        }

        private TimeSpan LastBreakOfRound()
        {
            var workoutsOfRound =
                _workoutPlan.Definition(
                        x => (Duration: x?.ToTimeSpan() ?? TimeSpan.Zero, IsBreak: false),
                        x => (Duration: x.ToTimeSpan(), IsBreak: false),
                        () => (Duration: TimeSpan.Zero, IsBreak: false),
                        x => (Duration: x.ToTimeSpan(), IsBreak: true))
                    .Round.Workouts;
            return workoutsOfRound.Aggregate(
                TimeSpan.Zero,
                (a, b) => b.IsBreak ? a.Add(b.Duration) : TimeSpan.Zero);
        }
    }
}