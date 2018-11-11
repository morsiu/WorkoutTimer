using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer.ExercisePlans;

namespace Timer.SoundEffects
{
    public sealed class SoundEffectsOfActions
    {
        private readonly IEnumerable<IAction> _actions;
        private readonly int _setCount;
        private readonly ISoundFactory _soundFactory;

        public SoundEffectsOfActions(IEnumerable<IAction> actions, int setCount, ISoundFactory soundFactory)
        {
            _actions = actions;
            _setCount = setCount;
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
                var actionSounds = new SoundsOfActions(_soundFactory);
                yield return actionSounds.WarmUp(TimeSpan.FromSeconds(15));
                foreach (var set in Sets())
                {
                    foreach (var action in _actions)
                    {
                        var soundEffect =
                            action.Map(
                                exercise: x => actionSounds.Exercise(x),
                                @break: x => actionSounds.Break(x),
                                invalid: () => null);
                        if (soundEffect != null)
                        {
                            yield return soundEffect;
                        }
                    }
                    if (set < _setCount)
                    {
                        yield return actionSounds.SetDone();
                    }
                }
                yield return actionSounds.AllDone();
            }

            Task DelayToAllowLastSoundToPlayOut() => Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            IEnumerable<int> Sets() => Enumerable.Range(2, _setCount);
        }
    }
}
