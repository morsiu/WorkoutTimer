using System.ComponentModel;
using WorkoutTimer.Planning.Visual;
using WorkoutTimer.Statistics.Visual;

namespace WorkoutTimer.Desktop
{
    internal sealed class TextualPlanningAndStatisticsOfWorkout
    {
        public TextualPlanningAndStatisticsOfWorkout(string? initialExpression)
        {
            Planning = new TextualPlanningOfWorkout(initialExpression);
            Statistics = new TextualStatisticsOfWorkout();
            Planning.PropertyChanged += OnPlanningPropertyChanged;
        }

        public TextualPlanningOfWorkout Planning { get; }

        public TextualStatisticsOfWorkout Statistics { get; }

        private void OnPlanningPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextualPlanningOfWorkout.ParsedWorkoutPlan))
                Statistics.WorkoutPlan = Planning.ParsedWorkoutPlan;
        }
    }
}