using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Timer.SoundEffects;
using Timer.SoundEffects.NAudio;
using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class RunWorkoutCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty NumberOfRoundsProperty =
            DependencyProperty.Register(
                nameof(NumberOfRounds),
                typeof(int),
                typeof(RunWorkoutCommand),
                new PropertyMetadata(1, NumberOfRoundsChanged, CoerceNumberOfRounds));

        private readonly CancelCommand _cancel = new CancelCommand();
        private readonly ModifyRoundCountCommand _addRound;
        private readonly ModifyRoundCountCommand _removeRound;
        private bool _running;

        public RunWorkoutCommand()
        {
            _removeRound = new ModifyRoundCountCommand(this, -1);
            _addRound = new ModifyRoundCountCommand(this, 1);
        }

        public event EventHandler CanExecuteChanged;
        
        public ICommand Cancel => _cancel;

        public ICommand AddRound => _addRound;

        public ICommand RemoveRound => _removeRound;

        public int NumberOfRounds
        {
            get => (int) GetValue(NumberOfRoundsProperty);
            set => SetValue(NumberOfRoundsProperty, value);
        }

        public bool CanExecute(object parameter) =>
            !_running && parameter is WorkoutRoundSteps && NumberOfRounds > 0;

        public async void Execute(object parameter)
        {
            if (_running || !(Workout(parameter) is Workout workout)) return;
            try
            {
                _running = true;
                var cancellation = new CancellationTokenSource();
                _cancel.Source = cancellation;
                RaiseCanExecuteChanged();
                await Execute(workout, cancellation.Token);
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


        private static object CoerceNumberOfRounds(DependencyObject d, object basevalue) =>
            (int)basevalue > 0 ? basevalue : 1;

        private static Task Execute(Workout workout, CancellationToken cancellationToken)
        {
            return RunSoundEffects();

            async Task RunSoundEffects()
            {
                using (var soundFactory = new NAudioSoundFactory())
                {
                    await new SoundEffectsOfWorkout(workout, soundFactory)
                        .Run(cancellationToken);
                }
            }
        }

        private static void NumberOfRoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RunWorkoutCommand self)) return;
            self._addRound.RaiseCanExecuteChanged();
            self._removeRound.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private Workout Workout(object parameter)
        {
            var round = (parameter as WorkoutRoundSteps)?.ToWorkoutRound();
            var roundCount = RoundCount.FromNumber(NumberOfRounds);
            return round != null && roundCount != null
                ? new Workout(round, roundCount.Value)
                : default;
        }

        private sealed class ModifyRoundCountCommand : ICommand
        {
            private readonly RunWorkoutCommand _target;
            private readonly int _delta;

            public ModifyRoundCountCommand(RunWorkoutCommand target, int delta)
            {
                _target = target;
                _delta = delta;
            }

            public bool CanExecute(object parameter) => _target.NumberOfRounds + _delta > 0;

            public void Execute(object parameter)
            {
                _target.NumberOfRounds += _delta;
                RaiseCanExecuteChanged();
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            public event EventHandler CanExecuteChanged;
        }
    }
}