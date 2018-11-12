using System;
using System.Windows;
using System.Windows.Input;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class WorkoutRoundCount : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(int),
                typeof(WorkoutRoundCount),
                new PropertyMetadata(1, ValueChanged, CoerceValue));

        public static readonly DependencyProperty RoundCountProperty;
        private static readonly DependencyPropertyKey RoundCountPropertyKey;

        private readonly ModifyRoundCountCommand _addRound;
        private readonly ModifyRoundCountCommand _removeRound;

        static WorkoutRoundCount()
        {
            RoundCountPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    "RoundCount",
                    typeof(RoundCount?),
                    typeof(WorkoutRoundCount),
                    new PropertyMetadata(WorkoutPlans.RoundCount.FromNumber(1), null, CoerceRoundCount));
            RoundCountProperty = RoundCountPropertyKey.DependencyProperty;
        }

        public WorkoutRoundCount()
        {
            _addRound = new ModifyRoundCountCommand(this, 1);
            _removeRound = new ModifyRoundCountCommand(this, -1);
        }

        public RoundCount? RoundCount
        {
            get => (RoundCount?) GetValue(RoundCountProperty);
            private set => SetValue(RoundCountPropertyKey, value);
        }

        public int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ICommand AddRound => _addRound;

        public ICommand RemoveRound => _removeRound;

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            return (int) basevalue > 0 ? basevalue : 1;
        }

        private static object CoerceRoundCount(DependencyObject d, object basevalue)
        {
            return d is WorkoutRoundCount self
                ? WorkoutPlans.RoundCount.FromNumber(self.Value)
                : null;
        }

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WorkoutRoundCount self)) return;
            self.CoerceValue(RoundCountPropertyKey.DependencyProperty);
            self._addRound.RaiseCanExecuteChanged();
            self._removeRound.RaiseCanExecuteChanged();
        }

        private sealed class ModifyRoundCountCommand : ICommand
        {
            private readonly int _delta;
            private readonly WorkoutRoundCount _target;

            public ModifyRoundCountCommand(WorkoutRoundCount target, int delta)
            {
                _target = target;
                _delta = delta;
            }

            public bool CanExecute(object parameter)
            {
                return _target.Value + _delta > 0;
            }

            public void Execute(object parameter)
            {
                _target.Value += _delta;
                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}