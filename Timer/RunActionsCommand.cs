using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Timer
{
    internal sealed class RunActionsCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty SetCountProperty =
            DependencyProperty.Register(
                nameof(SetCount),
                typeof(int),
                typeof(RunActionsCommand),
                new PropertyMetadata(1, SetCountChanged, CoerceSetCount));

        private static void SetCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RunActionsCommand self)) return;
            self._addSet.RaiseCanExecuteChanged();
            self._removeSet.RaiseCanExecuteChanged();
        }

        private static object CoerceSetCount(DependencyObject d, object basevalue) => (int)basevalue > 0 ? basevalue : 1;

        private readonly CancelCommand _cancel = new CancelCommand();
        private bool _running;
        private readonly ModifySetCountCommand _addSet;
        private readonly ModifySetCountCommand _removeSet;

        public RunActionsCommand()
        {
            _removeSet = new ModifySetCountCommand(this, -1);
            _addSet = new ModifySetCountCommand(this, 1);
        }

        public event EventHandler CanExecuteChanged;
        
        public ICommand Cancel => _cancel;

        public ICommand AddSet => _addSet;

        public ICommand RemoveSet => _removeSet;

        public int SetCount
        {
            get => (int) GetValue(SetCountProperty);
            set => SetValue(SetCountProperty, value);
        }

        public bool CanExecute(object parameter) => !_running && parameter is Actions;

        public async void Execute(object parameter)
        {
            if (_running || !(parameter is Actions actions)) return;
            try
            {
                _running = true;
                var cancellation = new CancellationTokenSource();
                _cancel.Source = cancellation;
                RaiseCanExecuteChanged();
                await Execute(actions, cancellation.Token);
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                _cancel.Source = null;
                _running = false;
                RaiseCanExecuteChanged();
            }
        }

        private Task Execute(Actions actions, CancellationToken cancellation) => actions.RunSoundEffects(SetCount, cancellation);

        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private sealed class ModifySetCountCommand : ICommand
        {
            private readonly RunActionsCommand _target;
            private readonly int _delta;

            public ModifySetCountCommand(RunActionsCommand target, int delta)
            {
                _target = target;
                _delta = delta;
            }

            public bool CanExecute(object parameter) => _target.SetCount + _delta > 0;

            public void Execute(object parameter)
            {
                _target.SetCount += _delta;
                RaiseCanExecuteChanged();
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            public event EventHandler CanExecuteChanged;
        }
    }
}