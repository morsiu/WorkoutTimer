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
        public static readonly DependencyProperty NumberOfSetsProperty =
            DependencyProperty.Register(
                nameof(NumberOfSets),
                typeof(int),
                typeof(RunWorkoutCommand),
                new PropertyMetadata(1, NumberOfSetsChanged, CoerceNumberOfSets));

        private readonly CancelCommand _cancel = new CancelCommand();
        private readonly ModifySetCountCommand _addSet;
        private readonly ModifySetCountCommand _removeSet;
        private bool _running;

        public RunWorkoutCommand()
        {
            _removeSet = new ModifySetCountCommand(this, -1);
            _addSet = new ModifySetCountCommand(this, 1);
        }

        public event EventHandler CanExecuteChanged;
        
        public ICommand Cancel => _cancel;

        public ICommand AddSet => _addSet;

        public ICommand RemoveSet => _removeSet;

        public int NumberOfSets
        {
            get => (int) GetValue(NumberOfSetsProperty);
            set => SetValue(NumberOfSetsProperty, value);
        }

        public bool CanExecute(object parameter) =>
            !_running && parameter is WorkoutRoundSteps && NumberOfSets > 0;

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


        private static object CoerceNumberOfSets(DependencyObject d, object basevalue) =>
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

        private static void NumberOfSetsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RunWorkoutCommand self)) return;
            self._addSet.RaiseCanExecuteChanged();
            self._removeSet.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private Workout Workout(object parameter)
        {
            var round = (parameter as WorkoutRoundSteps)?.ToWorkoutRound();
            var setCount = SetCount.FromNumber(NumberOfSets);
            return round != null && setCount != null
                ? new Workout(round, setCount.Value)
                : default;
        }

        private sealed class ModifySetCountCommand : ICommand
        {
            private readonly RunWorkoutCommand _target;
            private readonly int _delta;

            public ModifySetCountCommand(RunWorkoutCommand target, int delta)
            {
                _target = target;
                _delta = delta;
            }

            public bool CanExecute(object parameter) => _target.NumberOfSets + _delta > 0;

            public void Execute(object parameter)
            {
                _target.NumberOfSets += _delta;
                RaiseCanExecuteChanged();
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            public event EventHandler CanExecuteChanged;
        }
    }
}