using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class WorkoutCollection : Collection<Workout>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan
        {
            get
            {
                return a => this.Aggregate(a, (x, y) => y.AddTo(x));
            }
        }

        protected override void ClearItems()
        {
            foreach (var x in this)
            {
                x.PropertyChanged -= OnItemPropertyChanged;
            }
            base.ClearItems();
            RaiseWorkoutRoundChange();
        }

        protected override void InsertItem(int index, Workout item)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            base.InsertItem(index, item);
            RaiseWorkoutRoundChange();
        }

        protected override void RemoveItem(int index)
        {
            var item = Items[index];
            item.PropertyChanged -= OnItemPropertyChanged;
            base.RemoveItem(index);
            RaiseWorkoutRoundChange();
        }

        protected override void SetItem(int index, Workout item)
        {
            var oldItem = Items[index];
            oldItem.PropertyChanged -= OnItemPropertyChanged;
            item.PropertyChanged += OnItemPropertyChanged;
            base.SetItem(index, item);
            RaiseWorkoutRoundChange();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseWorkoutRoundChange();
        }

        private void RaiseWorkoutRoundChange()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkoutPlan)));
        }
    }
}