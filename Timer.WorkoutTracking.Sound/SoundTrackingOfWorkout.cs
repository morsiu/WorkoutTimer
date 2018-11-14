using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Sound
{
    public sealed class SoundTrackingOfWorkout
    {
        private readonly WorkoutPlan _workoutPlan;
        private readonly ISoundFactory _soundFactory;

        public SoundTrackingOfWorkout(WorkoutPlan workoutPlan, ISoundFactory soundFactory)
        {
            _workoutPlan = workoutPlan;
            _soundFactory = soundFactory;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            foreach (var x in SoundEffects())
            {
                await x.Play(cancellationToken);
            }
            await DelayToAllowLastSoundToPlayOut();

            IEnumerable<ISoundEffect> SoundEffects()
            {
                var sounds = new SoundsOfWorkout(_soundFactory);
                foreach (var step in
                    _workoutPlan.Select(
                        exercise: x => sounds.Exercise(x.ToTimeSpan()),
                        @break: x => sounds.Break(x.ToTimeSpan()),
                        warmUp: x => sounds.WarmUp(x.ToTimeSpan()),
                        nonLastRoundDone: () => sounds.RoundDone(),
                        lastRoundDone: () => sounds.WorkoutDone()))
                {
                    yield return step;
                }
            }

            Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
