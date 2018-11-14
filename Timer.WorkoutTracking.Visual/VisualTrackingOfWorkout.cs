using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    public sealed class VisualTrackingOfWorkout
    {
        private readonly WorkoutPlan _workoutPlan;
        private readonly IVisualWorkoutStatuses _workoutStatuses;

        public VisualTrackingOfWorkout(WorkoutPlan workoutPlan, IVisualWorkoutStatuses workoutStatuses)
        {
            _workoutPlan = workoutPlan;
            _workoutStatuses = workoutStatuses;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            foreach (var (durationOfStatus, status) in StatusesWithDurations())
            {
                status?.Apply();
                await Task.Delay(durationOfStatus, cancellationToken);
            }

            IEnumerable<(TimeSpan DurationOfStatus, IVisualWorkoutStatus Status)> StatusesWithDurations()
            {
                return
                    _workoutPlan.Select(
                        warmUp: x => (x.ToTimeSpan(), _workoutStatuses.WarmUp(x)),
                        exercise: (a, b) => (b.ToTimeSpan(), _workoutStatuses.Exercise(b, a)),
                        @break: (a, b) => (b.ToTimeSpan(), _workoutStatuses.Break(b, a)),
                        nonLastRoundDone: x => (TimeSpan.Zero, null),
                        lastRoundDone: x => (TimeSpan.Zero, _workoutStatuses.Done()));
            }
        }
    }
}
