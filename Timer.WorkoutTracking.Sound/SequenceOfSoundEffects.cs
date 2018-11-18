using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Timer.WorkoutTracking.Sound
{
    internal sealed class SequenceOfSoundEffects : ISoundEffect
    {
        private readonly IEnumerable<ISoundEffect> _soundEffects;

        public SequenceOfSoundEffects(IEnumerable<ISoundEffect> soundEffects)
        {
            _soundEffects = soundEffects;
        }

        public async Task Play(CancellationToken cancellationToken)
        {
            foreach (var soundEffect in _soundEffects)
            {
                await soundEffect.Play(cancellationToken);
            }
        }
    }
}