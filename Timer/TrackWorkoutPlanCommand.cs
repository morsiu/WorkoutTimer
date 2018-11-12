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
    internal sealed class TrackWorkoutPlanCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty RoundCountProperty =
            DependencyProperty.Register(
                nameof(RoundCount),
                typeof(RoundCount?),
                typeof(TrackWorkoutPlanCommand),
                new PropertyMetadata(null, RoundCountChanged));

        public static readonly DependencyProperty WorkoutRoundProperty =
            DependencyProperty.Register(
                nameof(WorkoutRound),
                typeof(WorkoutRound),
                typeof(TrackWorkoutPlanCommand),
                new PropertyMetadata(null, WorkoutRoundChanged));

        private readonly CancelCommand _cancel = new CancelCommand();
        private bool _running;

        public event EventHandler CanExecuteChanged;

        public ICommand Cancel => _cancel;

        public RoundCount? RoundCount
        {
            get => (RoundCount?) GetValue(RoundCountProperty);
            set => SetValue(RoundCountProperty, value);
        }

        public WorkoutRound WorkoutRound
        {
            get => (WorkoutRound) GetValue(WorkoutRoundProperty);
            set => SetValue(WorkoutRoundProperty, value);
        }

        public async void Execute(object parameter)
        {
            if (_running || !(WorkoutPlan() is WorkoutPlan workoutPlan)) return;
            try
            {
                _running = true;
                var cancellation = new CancellationTokenSource();
                _cancel.Source = cancellation;
                RaiseCanExecuteChanged();
                await Execute(workoutPlan, cancellation.Token);
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

            WorkoutPlan WorkoutPlan()
            {
                return WorkoutRound != null && RoundCount != null
                    ? new WorkoutPlan(WorkoutRound, RoundCount.Value)
                    : default;
            }
        }

        public bool CanExecute(object parameter) =>
            !_running && WorkoutRound != null && RoundCount != null;

        private static Task Execute(WorkoutPlan workoutPlan, CancellationToken cancellationToken)
        {
            return RunSoundEffects();

            async Task RunSoundEffects()
            {
                using (var soundFactory = new NAudioSoundFactory())
                {
                    await new SoundEffectsOfWorkout(workoutPlan, soundFactory)
                        .Run(cancellationToken);
                }
            }
        }

        private static void RoundCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackWorkoutPlanCommand self)
            {
                self.RaiseCanExecuteChanged();
            }
        }

        private static void WorkoutRoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackWorkoutPlanCommand self)
            {
                self.RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}