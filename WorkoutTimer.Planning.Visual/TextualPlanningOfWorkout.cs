using System;
using System.Linq;
using System.Windows;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Planning.Visual
{
    public sealed class TextualPlanningOfWorkout : DependencyObject
    {
        public static DependencyProperty ActualWorkoutExpressionProperty =
            DependencyProperty.RegisterReadOnly(
                    nameof(ActualWorkoutExpression),
                    typeof(string),
                    typeof(TextualPlanningOfWorkout),
                    new PropertyMetadata(string.Empty, null, CoerceActualWorkoutExpression))
                .DependencyProperty;
        public static DependencyProperty WorkoutExpressionProperty =
            DependencyProperty.Register(
                nameof(WorkoutExpression),
                typeof(string),
                typeof(TextualPlanningOfWorkout),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    WorkoutExpressionChanged));
        public static DependencyProperty WorkoutPlanProperty =
            DependencyProperty.RegisterReadOnly(
                    nameof(WorkoutPlan),
                    typeof(Func<WorkoutPlan, WorkoutPlan>),
                    typeof(TextualPlanningOfWorkout),
                    new PropertyMetadata(new Func<WorkoutPlan, WorkoutPlan>(x => x), WorkoutPlanChanged, CoerceWorkoutPlan))
                .DependencyProperty;

        public TextualPlanningOfWorkout()
        {
            CoerceValue(WorkoutPlanProperty);
        }

        public string ActualWorkoutExpression =>
            (string)GetValue(ActualWorkoutExpressionProperty);

        public string WorkoutExpression
        {
            get => (string)GetValue(WorkoutExpressionProperty);
            set => SetValue(WorkoutExpressionProperty, value);
        }

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan =>
            (Func<WorkoutPlan, WorkoutPlan>)GetValue(WorkoutPlanProperty);

        private static object CoerceActualWorkoutExpression(DependencyObject d, object baseValue)
        {
            if (d is not TextualPlanningOfWorkout self
                || self.WorkoutPlan?.Invoke(new WorkoutPlan()) is not { } workoutPlan) return baseValue;
            var definition = workoutPlan
                .Definition(
                    x => x is { } duration
                        ? $"{duration.TotalSeconds} W"
                        : string.Empty,
                    x => $"{x.TotalSeconds} E",
                    () => "E",
                    x => $"{x.TotalSeconds} B");
            return string.Format(
                "{0} {1} R ({2})",
                definition.Warmup,
                definition.Round.Number,
                definition.Round.Workouts.Any()
                    ? string.Join(" ", definition.Round.Workouts)
                    : "empty");
        }

        private static object CoerceWorkoutPlan(DependencyObject d, object baseValue)
        {
            if (d is not TextualPlanningOfWorkout self) return baseValue;
            var expression = new WorkoutPlanExpression(self.WorkoutExpression);
            return new Func<WorkoutPlan, WorkoutPlan>(expression.ToWorkoutPlan);
        }

        private static void WorkoutExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(WorkoutPlanProperty);
        }

        private static void WorkoutPlanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ActualWorkoutExpressionProperty);
        }
    }
}