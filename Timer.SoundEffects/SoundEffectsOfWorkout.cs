using System;
using System.Collections.Generic;
using System.Linq;
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
                yield return sounds.WarmUp(TimeSpan.FromSeconds(15));
                foreach (var set in Sets())
                {
                    foreach (var step in _workout.Steps)
                    {
                        var soundEffect =
                            step.Map(
                                exercise: x => sounds.Exercise(x.ToTimeSpan()),
                                @break: x => sounds.Break(x.ToTimeSpan()));
                        if (soundEffect != null)
                        {
                            yield return soundEffect;
                        }
                    }
                    if (set < _workout.SetCount)
                    {
                        yield return sounds.SetDone();
                    }
                }
                yield return sounds.AllDone();
            }

            Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            IEnumerable<int> Sets() => Enumerable.Range(2, _workout.SetCount);
        }
    }
}
