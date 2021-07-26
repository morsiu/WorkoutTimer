using System.ComponentModel;
using WorkoutTimer.Plans;
using WorkoutTimer.Visual;

namespace WorkoutTimer.Statistics.Visual
{
    public sealed class TextualStatisticsOfWorkout : INotifyPropertyChanged
    {
        private string _duration = string.Empty;
        private WorkoutPlan? _workoutPlan;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Duration
        {
            get => _duration;
            private set
            {
                _duration = value;
                PropertyChanged?.Invoke(this);
            }
        }

        public WorkoutPlan? WorkoutPlan
        {
            get => _workoutPlan;
            set
            {
                _workoutPlan = value;
                UpdateDuration();
            }
        }

        private void UpdateDuration()
        {
            if (WorkoutPlan is null)
            {
                Duration = string.Empty;
                return;
            }
            var statistics = new WorkoutDurationStatistics(WorkoutPlan);
            Duration = $"Total: {statistics.Total()}, Exercise per round: {statistics.ExercisePerRound()}";
        }
    }
}