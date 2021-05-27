using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Tracking.Visual
{
    internal sealed class WorkoutSegment : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly WorkoutsOfPlan _workoutsOfPlan;
        private List<IWorkout>? _items;
        private int? _round;

        public WorkoutSegment(WorkoutsOfPlan workoutsOfPlan)
        {
            _workoutsOfPlan = workoutsOfPlan;
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int? Round
        {
            get => _round;
            set
            {
                _round = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Round)));
            }
        }

        public void Clear()
        {
            _items = null;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            Round = null;
        }

        public void Remove(IWorkout workout)
        {
            if (_items?.IndexOf(workout) is { } index)
            {
                _items.RemoveAt(index);
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] {workout}, index));
            }
        }

        public void SwitchToRound(Round round)
        {
            _items = _workoutsOfPlan.WorkoutsOfRound(round).ToList();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            Round = round.Number;
        }

        public IEnumerator GetEnumerator()
        {
            return (_items ?? Enumerable.Empty<object>()).GetEnumerator();
        }
    }
}