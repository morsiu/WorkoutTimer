using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WorkoutTimer.Plans;
using WorkoutTimer.Tracking;
using WorkoutTimer.Tracking.Sound;
using WorkoutTimer.Tracking.Sound.NAudio;
using WorkoutTimer.Tracking.Visual;

namespace WorkoutTimer.Desktop
{
    internal sealed class TrackWorkoutPlanCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty WorkoutPlanProperty =
            DependencyProperty.Register(
                nameof(WorkoutPlan),
                typeof(Func<WorkoutPlan, WorkoutPlan>),
                typeof(TrackWorkoutPlanCommand),
                new PropertyMetadata(null, WorkoutPlanChanged));

        public static readonly DependencyProperty WorkoutsOfCurrentSegmentProperty;
        private static readonly DependencyPropertyKey WorkoutsOfCurrentSegmentPropertyKey;

        private readonly CancelCommand _cancel = new();
        private bool _running;

        static TrackWorkoutPlanCommand()
        {
            WorkoutsOfCurrentSegmentPropertyKey = 
                DependencyProperty.RegisterReadOnly(
                    nameof(WorkoutsOfCurrentSegment),
                    typeof(object),
                    typeof(TrackWorkoutPlanCommand),
                    new PropertyMetadata(null));
            WorkoutsOfCurrentSegmentProperty = WorkoutsOfCurrentSegmentPropertyKey.DependencyProperty;
        }

        public event EventHandler? CanExecuteChanged;

        public ICommand Cancel => _cancel;

        public Func<WorkoutPlan, WorkoutPlan>? WorkoutPlan
        {
            get => (Func<WorkoutPlan, WorkoutPlan>?) GetValue(WorkoutPlanProperty);
            set => SetValue(WorkoutPlanProperty, value);
        }

        public object? WorkoutsOfCurrentSegment
        {
            get => GetValue(WorkoutsOfCurrentSegmentProperty);
            private set => SetValue(WorkoutsOfCurrentSegmentPropertyKey, value);
        }

        public async void Execute(object? parameter)
        {
            if (_running || !(WorkoutPlan?.Invoke(new WorkoutPlan()) is { } workoutPlan)) return;
            try
            {
                _running = true;
                var cancellation = new CancellationTokenSource();
                _cancel.Source = cancellation;
                RaiseCanExecuteChanged();
                await RunTracking();

                async Task RunTracking()
                {
                    try
                    {
                        var trackedWorkoutPlan = new TrackedWorkoutPlan(workoutPlan);
                        using var soundFactory = new NAudioSoundFactory();
                        await using var soundTracking = new SoundTrackingOfWorkout(trackedWorkoutPlan, soundFactory);
                        using var visualTracking = new VisualTrackingOfWorkout(trackedWorkoutPlan, workoutPlan);
                        WorkoutsOfCurrentSegment = visualTracking.WorkoutsOfCurrentRound;
                        await trackedWorkoutPlan.Start(cancellation.Token);
                    }
                    finally
                    {
                        WorkoutsOfCurrentSegment = null;
                    }
                }
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

        public bool CanExecute(object? parameter) =>
            !_running && WorkoutPlan != null;

        private static void WorkoutPlanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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