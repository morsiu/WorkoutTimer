using System.Windows;
using System.Windows.Controls;
using Timer.WorkoutPlans;
using Duration = Timer.WorkoutPlans.Duration;

namespace Timer.WorkoutTracking.Visual
{
    [TemplateVisualState(GroupName = "WorkoutStates", Name = "Idle")]
    [TemplateVisualState(GroupName = "WorkoutStates", Name = "WarmUp")]
    [TemplateVisualState(GroupName = "WorkoutStates", Name = "Exercise")]
    [TemplateVisualState(GroupName = "WorkoutStates", Name = "Break")]
    public sealed class CurrentWorkout : Control, IVisualWorkoutStatuses
    {
        public static readonly DependencyProperty DurationProperty;
        public static readonly DependencyProperty RoundProperty;
        private static readonly DependencyPropertyKey DurationPropertyKey;
        private static readonly DependencyPropertyKey RoundPropertyKey;

        static CurrentWorkout()
        {
            DurationPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    nameof(Duration),
                    typeof(System.Windows.Duration?),
                    typeof(CurrentWorkout),
                    new PropertyMetadata());
            DurationProperty = DurationPropertyKey.DependencyProperty;
            RoundPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    nameof(Round),
                    typeof(int?),
                    typeof(CurrentWorkout),
                    new PropertyMetadata());
            RoundProperty = RoundPropertyKey.DependencyProperty;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, "Idle", useTransitions: true);
        }

        public System.Windows.Duration? Duration
        {
            get => (System.Windows.Duration?) GetValue(DurationProperty);
            private set => SetValue(DurationPropertyKey, value);
        }

        public int? Round
        {
            get => (int) GetValue(RoundProperty);
            private set => SetValue(RoundPropertyKey, value);
        }

        public IVisualWorkoutStatus WarmUp(Duration duration)
        {
            return new VisualStateOfWorkout(this, "WarmUp", duration, round: null);
        }

        public IVisualWorkoutStatus Exercise(Duration duration, Round round)
        {
            return new VisualStateOfWorkout(this, "Exercise", duration, round);
        }

        public IVisualWorkoutStatus Break(Duration duration, Round round)
        {
            return new VisualStateOfWorkout(this, "Break", duration, round);
        }

        public IVisualWorkoutStatus Done()
        {
            return new VisualStateOfWorkout(this, "Idle", duration: null, round: null);
        }

        private sealed class VisualStateOfWorkout : IVisualWorkoutStatus
        {
            private readonly CurrentWorkout _target;
            private readonly string _visualStateName;
            private readonly Duration? _duration;
            private readonly Round? _round;

            public VisualStateOfWorkout(
                CurrentWorkout target,
                string visualStateName,
                Duration? duration,
                Round? round)
            {
                _target = target;
                _visualStateName = visualStateName;
                _duration = duration;
                _round = round;
            }

            public void Apply()
            {
                _target.Dispatcher.InvokeAsync(
                    () =>
                    {
                        VisualStateManager.GoToState(_target, "Idle", useTransitions: false);
                        _target.Duration =
                            _duration != null
                                ? new System.Windows.Duration(_duration.Value.ToTimeSpan())
                                : default(System.Windows.Duration?);
                        _target.Round = _round?.Number;
                        VisualStateManager.GoToState(_target, _visualStateName, useTransitions: true);
                    });
            }
        }
    }
}
