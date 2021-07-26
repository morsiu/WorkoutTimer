using System;
using System.Threading;
using System.Windows.Input;
using WorkoutTimer.Plans;
using WorkoutTimer.Visual;

namespace WorkoutTimer.Tracking.Visual
{
    public sealed class VisualTrackingOfWorkout : IDisposable
    {
        private readonly IDisposable _trackedWorkoutPlanSubscription;
        private readonly WorkoutsOfPlan _workoutsOfPlan;
        private readonly WorkoutSegment _workoutSegment;

        public VisualTrackingOfWorkout(
            TrackedWorkoutPlan trackedWorkoutPlan,
            WorkoutPlan workoutPlan,
            CancellationTokenSource trackingCancellation)
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
            Cancel =
                new OneOffCommand(
                    new DelegateCommand(
                        () => trackingCancellation.Cancel(),
                        () => !trackingCancellation.IsCancellationRequested));
            WorkoutsOfCurrentRound = workoutSegment;
        }

        public ICommand Cancel { get; }

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
            _workoutSegment.Remove(workout);
        }

        private void OnWorkoutStart(ITrackedWorkout trackedWorkout, CancellationToken _)
        {
            var workout = _workoutsOfPlan.Workout(trackedWorkout);
            var complete = trackedWorkout.Match(
                (_, _, _) => default(Action?),
                (_, _, _) => default,
                (_, _, x) => x,
                _ => default);
            workout.Activate(complete);
        }

        public void Dispose()
        {
            _trackedWorkoutPlanSubscription.Dispose();
        }
    }
}
