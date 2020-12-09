using System.Collections.Immutable;

namespace WorkoutTimer.Tracking.Sound
{
    internal static class SoundEffectExtensions
    {
        public static ISoundEffect Then(this ISoundEffect first, ISoundEffect second)
        {
            return new SequenceOfSoundEffects(ImmutableArray.Create(first, second));
        }
    }
}