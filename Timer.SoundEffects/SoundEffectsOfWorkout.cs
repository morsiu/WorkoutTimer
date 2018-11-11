using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;

namespace Timer.SoundEffects
{
    public sealed class SoundEffectsOfWorkout
    {
        private readonly Workout _workout;
        private readonly ISoundFactory _soundFactory;

        public SoundEffectsOfWorkout(Workout workout, ISoundFactory soundFactory)
        {
            _workout = workout;
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
                var sounds = new SoundsOfWorkoutSteps(_soundFactory);
                foreach (var step in _workout.Steps())
                {
                    yield return
                        step.Map(
                            exercise: x => sounds.Exercise(x.ToTimeSpan()),
                            @break: x => sounds.Break(x.ToTimeSpan()),
                            warmUp: x => sounds.WarmUp(x.ToTimeSpan()),
                            setDone: () => sounds.SetDone(),
                            workoutDone: () => sounds.WorkoutDone());
                }
            }

            Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
