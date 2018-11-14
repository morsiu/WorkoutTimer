using System.Windows.Controls;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    public partial class CurrentWorkout : IVisualWorkoutStatuses
    {
        public CurrentWorkout()
        {
            InitializeComponent();
        }

        public IVisualWorkoutStatus WarmUp(Duration duration)
        {
            return new Status(this, $"Warmup! {duration.TotalSeconds}s left!");
        }

        public IVisualWorkoutStatus Exercise(Duration duration, Round round)
        {
            return new Status(this, $"Exercise! {duration.TotalSeconds}s left! Round #{round.Number}");
        }

        public IVisualWorkoutStatus Break(Duration duration, Round round)
        {
            return new Status(this, $"Break! {duration.TotalSeconds}s left! Round #{round.Number}");
        }

        public IVisualWorkoutStatus Done()
        {
            return new Status(this, "Done :)");
        }

        private sealed class Status : IVisualWorkoutStatus
        {
            private readonly CurrentWorkout _target;
            private readonly string _text;

            public Status(CurrentWorkout target, string text)
            {
                _target = target;
                _text = text;
            }

            public void Apply()
            {
                _target.Dispatcher.InvokeAsync(
                    () => _target.StatusControl.Text = _text);
            }
        }
    }
}
