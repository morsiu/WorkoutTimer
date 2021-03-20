using System;
using System.Windows;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Statistics.Visual
{
    public sealed class TextualStatisticsOfWorkout : DependencyObject
    {
        public static readonly DependencyProperty WorkoutDurationStatisticsProperty =
            DependencyProperty.RegisterReadOnly(
                    nameof(WorkoutDurationStatistics),
                    typeof(string),
                    typeof(TextualStatisticsOfWorkout),
                    new PropertyMetadata(string.Empty, null, CoerceWorkoutDurationStatistics))
                .DependencyProperty;

        public static readonly DependencyProperty WorkoutPlanProperty =
            DependencyProperty.Register(
                nameof(WorkoutPlan),
                typeof(Func<WorkoutPlan, WorkoutPlan>),
                typeof(TextualStatisticsOfWorkout),
                new PropertyMetadata(new Func<WorkoutPlan, WorkoutPlan>(x => x), WorkoutPlanChanged, CoerceWorkoutPlan));

        public TextualStatisticsOfWorkout()
        {
            CoerceValue(WorkoutDurationStatisticsProperty);
        }

        public string WorkoutDurationStatistics =>
            (string)GetValue(WorkoutDurationStatisticsProperty);

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan
        {
            get => (Func<WorkoutPlan, WorkoutPlan>)GetValue(WorkoutPlanProperty);
            set => SetValue(WorkoutPlanProperty, value);
        }

        private static object CoerceWorkoutDurationStatistics(DependencyObject d, object baseValue)
        {
            if (d is not TextualStatisticsOfWorkout self) return baseValue;
            var workoutPlan = self.WorkoutPlan(new WorkoutPlan());
            var workoutDurationStatistics = new WorkoutDurationStatistics(workoutPlan);
            return $"Total: {workoutDurationStatistics.Total()}, Exercise per round: {workoutDurationStatistics.ExercisePerRound()}";
        }

        private static object CoerceWorkoutPlan(DependencyObject d, object baseValue)
        {
            return baseValue is null ? new Func<WorkoutPlan, WorkoutPlan>(x => x) : baseValue;
        }

        private static void WorkoutPlanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(WorkoutDurationStatisticsProperty);
        }
    }
}
