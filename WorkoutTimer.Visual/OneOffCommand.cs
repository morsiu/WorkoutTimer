using System;
using System.Windows.Input;

namespace WorkoutTimer.Visual
{
    public sealed class OneOffCommand : ICommand
    {
        private readonly ICommand _command;
        private bool _executed;

        public OneOffCommand(ICommand command)
        {
            _command = command;
            _command.CanExecuteChanged += OnCanExecuteChanged;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !_executed && _command.CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_executed)
            {
                return;
            }
            _executed = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            _command.CanExecuteChanged -= OnCanExecuteChanged;
            _command.Execute(parameter);
        }

        private void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}