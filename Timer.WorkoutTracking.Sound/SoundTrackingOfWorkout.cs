using System;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Sound
{
    public sealed class SoundTrackingOfWorkout : IAsyncDisposable
    {
        private readonly IDisposable _planSubscription;
        private readonly SoundsOfWorkout _sounds;

        public SoundTrackingOfWorkout(TrackedWorkoutPlan plan, ISoundFactory soundFactory)
        {
            _sounds = new SoundsOfWorkout(soundFactory);
            _planSubscription = plan.Subscribe(
                new TrackedWorkoutPlanVisitor()
                    .OnWorkoutStart(OnWorkoutStart)
                    .OnRoundEnd(OnRoundEnd));
        }

        private void OnRoundEnd(Round round, CancellationToken cancellationToken)
        {
            var sound = round.IsLast
                ? _sounds.WorkoutDone()
                : _sounds.RoundDone();
            _ = sound.Play(cancellationToken);
        }

        private void OnWorkoutStart(ITrackedWorkout workout, CancellationToken cancellationToken)
        {
            var sound = workout.Match(
                (a, _, b) => _sounds.Break(b.ToTimeSpan()),
                (a, _, b) => _sounds.Exercise(b.ToTimeSpan()),
                x => _sounds.WarmUp(x.ToTimeSpan()));
            _ = sound.Play(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DelayToAllowLastSoundToPlayOut();
            _planSubscription.Dispose();

            static Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
