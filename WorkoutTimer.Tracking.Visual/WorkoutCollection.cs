using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WorkoutTimer.Tracking.Visual
{
    [TemplatePart(Name = "PART_Items", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_Round", Type = typeof(ContentControl))]
    public sealed class WorkoutCollection : Control
    {
        public static DependencyProperty RoundProperty =
            DependencyProperty.Register(
                nameof(Round),
                typeof(int?),
                typeof(WorkoutCollection),
                new PropertyMetadata(OnRoundPropertyChanged));
        public static DependencyProperty WorkoutsProperty =
            DependencyProperty.Register(nameof(Workouts), typeof(IEnumerable), typeof(WorkoutCollection));
        private ContentControl? _roundPart;

        public int? Round
        {
            get => (int?)GetValue(RoundProperty);
            set => SetValue(RoundProperty, value);
        }

        public IEnumerable Workouts
        {
            get => (IEnumerable)GetValue(WorkoutsProperty);
            set => SetValue(WorkoutsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template.FindName("PART_Items", this) is ItemsControl itemsPart)
            {
                BindingOperations.SetBinding(
                    itemsPart,
                    ItemsControl.ItemsSourceProperty,
                    new Binding(nameof(Workouts)) { RelativeSource = RelativeSource.TemplatedParent });
            }
            _roundPart = null;
            if (Template.FindName("PART_Round", this) is ContentControl roundPart)
            {
                UpdateRoundPart(roundPart, Round);
                _roundPart = roundPart;
            }
        }

        private static void OnRoundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (WorkoutCollection)d;
            if (self._roundPart is { } roundPart)
            {
                var round = (int?)e.NewValue;
                UpdateRoundPart(roundPart, round);
            }
        }

        private static void UpdateRoundPart(ContentControl roundPart, int? round)
        {
            roundPart.Content = round;
            roundPart.Visibility = round is null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
