using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WorkoutTimer.Plans;
using Duration = WorkoutTimer.Plans.Duration;

namespace WorkoutTimer.Tracking.Visual
{
    [TemplatePart(Name = "PART_Countdown", Type = typeof(ProgressBar))]
    [TemplatePart(Name = "PART_Description", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Round", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Complete", Type = typeof(ButtonBase))]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Active")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Inactive")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "InactiveAgain")]
    [TemplateVisualState(GroupName = "CompletionStates", Name = "AutomaticCompletion")]
    [TemplateVisualState(GroupName = "CompletionStates", Name = "ManualCompletion")]
    public sealed class Workout : Control, IWorkout
    {
        public static readonly DependencyProperty DurationProperty;
        public static readonly DependencyProperty RoundProperty;
        private static readonly DependencyPropertyKey DescriptionPropertyKey;
        private static readonly DependencyPropertyKey DurationPropertyKey;
        private static readonly DependencyPropertyKey RoundPropertyKey;
        private static readonly DependencyProperty DescriptionProperty;
        private Action? _complete;
        private ButtonBase? _completePart;

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
                    typeof(System.Windows.Duration),
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

        internal Workout(WorkoutType type, Duration? duration, Round? round, bool useManualCompletion)
        {
            Description = DescriptionOfWorkoutType();
            Duration = duration?.ToTimeSpan() ?? TimeSpan.Zero;
            Round = round?.Number;
            GoToState("Inactive");
            GoToState(useManualCompletion ? "ManualCompletion" : "AutomaticCompletion");

            string DescriptionOfWorkoutType()
            {
                return type switch
                {
                    WorkoutType.WarmUp => "Warm up",
                    WorkoutType.Exercise => "Exercise",
                    WorkoutType.Break => "Break",
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                };
            }
        }

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            private set => SetValue(DescriptionPropertyKey, value);
        }

        public System.Windows.Duration Duration
        {
            get => (System.Windows.Duration) GetValue(DurationProperty);
            private set => SetValue(DurationPropertyKey, value);
        }

        public int? Round
        {
            get => (int?) GetValue(RoundProperty);
            private set => SetValue(RoundPropertyKey, value);
        }

        void IWorkout.Activate(Action? complete)
        {
            GoToState("Active");
            _complete = complete;
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
            if (_completePart is not null)
            {
                _completePart.Click -= OnComplete;
                _completePart = null;
            }
            if (Template?.FindName("PART_Complete", this) is ButtonBase complete)
            {
                complete.Click += OnComplete;
                _completePart = complete;
            }
        }

        private void OnComplete(object sender, RoutedEventArgs e)
        {
            _complete?.Invoke();
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
