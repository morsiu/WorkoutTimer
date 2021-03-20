using System;
using System.Threading;
using System.Windows.Input;

namespace WorkoutTimer.Desktop
{
    internal sealed class CancelCommand : ICommand
    {
        private CancellationTokenSource? _source;
        
        public event EventHandler? CanExecuteChanged;

        public CancellationTokenSource? Source
        {
            get => _source;
            set
            {
                _source = value;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(object? parameter) => Source != null && !Source.IsCancellationRequested;

        public void Execute(object? parameter)
        {
            Source?.Cancel();
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}