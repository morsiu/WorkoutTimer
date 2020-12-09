using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Tracking.Visual
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

        public void Clear()
        {
            _round = null;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void SwitchToRound(Round round)
        {
            _round = round;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator GetEnumerator()
        {
            var workouts = 
                _round is { } round
                    ? _workoutsOfPlan.WorkoutsOfRound(round)
                    : Enumerable.Empty<IWorkout>();
            return workouts.GetEnumerator();
        }
    }
}