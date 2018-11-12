using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class WorkoutCollection : Collection<Workout>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WorkoutRound WorkoutRound
        {
            get
            {
                var round =
                    this.Aggregate(
                        new WorkoutRound(),
                        (x, y) => y.AddTo(x));
                return round;
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            RaiseWorkoutRoundChange();
        }

        protected override void InsertItem(int index, Workout item)
        {
            base.InsertItem(index, item);
            RaiseWorkoutRoundChange();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            RaiseWorkoutRoundChange();
        }

        protected override void SetItem(int index, Workout item)
        {
            base.SetItem(index, item);
            RaiseWorkoutRoundChange();
        }

        private void RaiseWorkoutRoundChange()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkoutRound)));
        }
    }
}