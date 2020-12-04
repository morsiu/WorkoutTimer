using System;
using System.Threading;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking
{
    public sealed class TrackedWorkoutPlanVisitor
    {
        private readonly Action<ITrackedWorkout, CancellationToken> _workoutStart;
        private readonly Action<ITrackedWorkout, CancellationToken> _workoutEnd;
        private readonly Action<Round, CancellationToken> _roundStart;
        private readonly Action<Round, CancellationToken> _roundEnd;

        public TrackedWorkoutPlanVisitor()
        {
        }

        private TrackedWorkoutPlanVisitor(
            Action<ITrackedWorkout, CancellationToken> workoutStart,
            Action<ITrackedWorkout, CancellationToken> workoutEnd,
            Action<Round, CancellationToken> roundStart,
            Action<Round, CancellationToken> roundEnd)
        {
            _workoutStart = workoutStart;
            _workoutEnd = workoutEnd;
            _roundStart = roundStart;
            _roundEnd = roundEnd;
        }

        public TrackedWorkoutPlanVisitor OnWorkoutStart(Action<ITrackedWorkout, CancellationToken> action) =>
            new(action, _workoutEnd, _roundStart, _roundEnd);

        public TrackedWorkoutPlanVisitor OnWorkoutEnd(Action<ITrackedWorkout, CancellationToken> action) =>
            new(_workoutStart, action, _roundStart, _roundEnd);

        public TrackedWorkoutPlanVisitor OnRoundStart(Action<Round, CancellationToken> action) =>
            new(_workoutStart, _workoutEnd, action, _roundEnd);

        public TrackedWorkoutPlanVisitor OnRoundEnd(Action<Round, CancellationToken> action) =>
            new(_workoutStart, _workoutEnd, _roundStart, action);

        internal void VisitWorkoutStart(ITrackedWorkout workout, CancellationToken cancellationToken)
        {
            _workoutStart?.Invoke(workout, cancellationToken);
        }

        internal void VisitWorkoutEnd(ITrackedWorkout workout, CancellationToken cancellationToken)
        {
            _workoutEnd?.Invoke(workout, cancellationToken);
        }

        internal void VisitRoundStart(Round round, CancellationToken cancellationToken)
        {
            _roundStart?.Invoke(round, cancellationToken);
        }

        internal void VisitRoundEnd(Round round, CancellationToken cancellationToken)
        {
            _roundEnd?.Invoke(round, cancellationToken);
        }
    }
}
