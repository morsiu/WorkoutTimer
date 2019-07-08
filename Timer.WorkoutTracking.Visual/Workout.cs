using System;
using System.Windows;
using System.Windows.Controls;
using Timer.WorkoutPlans;
using Duration = Timer.WorkoutPlans.Duration;

namespace Timer.WorkoutTracking.Visual
{
    [TemplatePart(Name = "PART_Countdown", Type = typeof(ProgressBar))]
    [TemplatePart(Name = "PART_Description", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Round", Type = typeof(TextBlock))]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Active")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Inactive")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "InactiveAgain")]
    public sealed class Workout : Control, IWorkout
    {
        public static readonly DependencyProperty DurationProperty;
        public static readonly DependencyProperty RoundProperty;
        private static readonly DependencyPropertyKey DescriptionPropertyKey;
        private static readonly DependencyPropertyKey DurationPropertyKey;
        private static readonly DependencyPropertyKey RoundPropertyKey;
        private static readonly DependencyProperty DescriptionProperty;

        static Workout()
        {
            DescriptionPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    nameof(Description),
                    typeof(string),
                    typeof(Workout),
                    new PropertyMetadata());
            DescriptionProperty = DescriptionPropertyKey.DependencyProperty;
            DurationPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    nameof(Duration),
                    typeof(System.Windows.Duration?),
                    typeof(Workout),
                    new PropertyMetadata());
            DurationProperty = DurationPropertyKey.DependencyProperty;
            RoundPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    nameof(Round),
                    typeof(int?),
                    typeof(Workout),
                    new PropertyMetadata());
            RoundProperty = RoundPropertyKey.DependencyProperty;
        }

        internal Workout(WorkoutType type, Duration? duration, Round? round)
        {
            Description = DescriptionOfWorkoutType();
            Duration = duration?.ToTimeSpan();
            Round = round?.Number;
            GoToState("Inactive");

            string DescriptionOfWorkoutType()
            {
                switch (type)
                {
                    case WorkoutType.WarmUp:
                        return "Warm up";
                    case WorkoutType.Exercise:
                        return "Exercise";
                    case WorkoutType.Break:
                        return "Break";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            private set => SetValue(DescriptionPropertyKey, value);
        }

        public System.Windows.Duration? Duration
        {
            get => (System.Windows.Duration?) GetValue(DurationProperty);
            private set => SetValue(DurationPropertyKey, value);
        }

        public int? Round
        {
            get => (int?) GetValue(RoundProperty);
            private set => SetValue(RoundPropertyKey, value);
        }

        void IWorkout.Activate()
        {
            GoToState("Active");
        }

        void IWorkout.Deactivate()
        {
            GoToState("InactiveAgain");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template?.FindName("PART_Description", this) is TextBlock description)
            {
                description.Text = Description;
            }
            if (Template?.FindName("PART_Round", this) is TextBlock round)
            {
                round.Text = Round?.ToString();
            }
        }

        private void GoToState(string visualState)
        {
            if (IsLoaded)
            {
                GoToState();
            }
            else
            {
                Loaded += OnLoaded;

                void OnLoaded(object sender, RoutedEventArgs e)
                {
                    GoToState();
                    Loaded -= OnLoaded;
                }
            }

            void GoToState()
            {
                VisualStateManager.GoToState(this, visualState, useTransitions: true);
            }
        }
    }
}
