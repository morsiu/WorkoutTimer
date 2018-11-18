using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    internal sealed class WorkoutSegment : IEnumerable, INotifyCollectionChanged
    {
        private readonly WorkoutsOfPlan _workoutsOfPlan;
        private Round? _round;

        public WorkoutSegment(WorkoutsOfPlan workoutsOfPlan)
        {
            _workoutsOfPlan = workoutsOfPlan;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void SwitchToRound(Round round)
        {
            _round = round;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator GetEnumerator()
        {
            var workouts = _round != null
                ? _workoutsOfPlan.OfRound(_round.Value).Select(x => x.Workout)
                : Enumerable.Empty<IWorkout>();
            return workouts.GetEnumerator();
        }
    }
}