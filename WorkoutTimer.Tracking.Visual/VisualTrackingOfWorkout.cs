using System;
using System.Threading;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Tracking.Visual
{
    public sealed class VisualTrackingOfWorkout : IDisposable
    {
        private readonly IDisposable _trackedWorkoutPlanSubscription;
        private readonly WorkoutsOfPlan _workoutsOfPlan;
        private readonly WorkoutSegment _workoutSegment;

        public VisualTrackingOfWorkout(TrackedWorkoutPlan trackedWorkoutPlan, WorkoutPlan workoutPlan)
        {
            var workoutsOfPlan = new WorkoutsOfPlan(workoutPlan);
            var workoutSegment = new WorkoutSegment(workoutsOfPlan);
            _trackedWorkoutPlanSubscription =
                trackedWorkoutPlan.Subscribe(
                    new TrackedWorkoutPlanVisitor()
                        .OnWorkoutStart(OnWorkoutStart)
                        .OnWorkoutEnd(OnWorkoutEnd)
                        .OnRoundStart(OnRoundStart)
                        .OnRoundEnd(OnRoundEnd));
            _workoutsOfPlan = workoutsOfPlan;
            _workoutSegment = workoutSegment;
            WorkoutsOfCurrentRound = workoutSegment;
        }

        public object WorkoutsOfCurrentRound { get; }

        private void OnRoundEnd(Round round, CancellationToken _)
        {
            _workoutSegment.Clear();
        }

        private void OnRoundStart(Round round, CancellationToken _)
        {
            _workoutSegment.SwitchToRound(round);
        }

        private void OnWorkoutEnd(ITrackedWorkout trackedWorkout, CancellationToken _)
        {
            var workout = _workoutsOfPlan.Workout(trackedWorkout);
            workout.Deactivate();
        }

        private void OnWorkoutStart(ITrackedWorkout trackedWorkout, CancellationToken _)
        {
            var workout = _workoutsOfPlan.Workout(trackedWorkout);
            var complete = trackedWorkout.Match((_, _, _) => null, (_, _, _) => null, (_, _, x) => x, _ => null);
            workout.Activate(complete);
        }

        public void Dispose()
        {
            _trackedWorkoutPlanSubscription.Dispose();
        }
    }
}
