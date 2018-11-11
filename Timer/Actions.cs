using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer.SoundEffects;
using Timer.SoundEffects.NAudio;

namespace Timer
{
    internal sealed class Actions : Collection<Action>
    {
        public async Task RunSoundEffects(int setCount, CancellationToken cancellationToken)
        {
            using (var soundFactory = new NAudioSoundFactory())
            {
                await new SoundEffectsOfActions(this.ToList(), setCount, soundFactory).Run(cancellationToken);
            }
        }
    }
}