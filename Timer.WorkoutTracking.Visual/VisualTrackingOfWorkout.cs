using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    public sealed class VisualTrackingOfWorkout
    {
        private readonly Dispatcher _dispatcher;
        private readonly WorkoutsOfPlan _workoutsOfPlan;
        private readonly WorkoutSegment _workoutSegment;

        public VisualTrackingOfWorkout(WorkoutPlan workoutPlan, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            var workoutsOfPlan = new WorkoutsOfPlan(workoutPlan);
            var workoutSegment = new WorkoutSegment(workoutsOfPlan);
            _workoutsOfPlan = workoutsOfPlan;
            _workoutSegment = workoutSegment;
            WorkoutsOfCurrentRound = workoutSegment;
        }

        public object WorkoutsOfCurrentRound { get; }

        public async Task Run(CancellationToken cancellationToken)
        {
            foreach (var round in _workoutsOfPlan.Rounds())
            {
                 _workoutSegment.SwitchToRound(round);
                foreach (var (duration, workout) in _workoutsOfPlan.OfRound(round))
                {
                    workout.Activate();
                    await Task.Delay(duration.ToTimeSpan(), cancellationToken);
                    workout.Deactivate();
                }
            }
        }
    }
}
