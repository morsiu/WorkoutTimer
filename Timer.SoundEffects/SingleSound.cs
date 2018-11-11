using System.Threading;
using System.Threading.Tasks;

namespace Timer.SoundEffects
{
    internal sealed class SingleSound : ISoundEffect
    {
        private readonly ISound _sound;

        public SingleSound(ISound sound)
        {
            _sound = sound;
        }

        public Task Play(CancellationToken cancellationToken)
        {
            _sound.PlayAsynchronously();
            return Task.CompletedTask;
        }
    }
}