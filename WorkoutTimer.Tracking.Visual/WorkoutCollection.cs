using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WorkoutTimer.Tracking.Visual
{
    [TemplatePart(Name = "PART_Items", Type = typeof(ItemsControl))]
    public sealed class WorkoutCollection : Control
    {
        public static DependencyProperty WorkoutsProperty =
            DependencyProperty.Register(nameof(Workouts), typeof(IEnumerable), typeof(WorkoutCollection));

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
        }
    }
}
