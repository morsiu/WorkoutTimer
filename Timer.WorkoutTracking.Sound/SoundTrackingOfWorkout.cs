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
                return _workoutPlan.EnumerateLinearly(
                    new WorkoutPlanVisitor<ISoundEffect>()
                        .OnWarmup(x => sounds.WarmUp(x.ToTimeSpan()))
                        .OnExercise((a, b) => sounds.Exercise(b.ToTimeSpan()))
                        .OnBreak((a, b) => sounds.Break(b.ToTimeSpan()))
                        .OnRoundDone(x => sounds.RoundDone(), x => sounds.WorkoutDone()));
            }

            Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
