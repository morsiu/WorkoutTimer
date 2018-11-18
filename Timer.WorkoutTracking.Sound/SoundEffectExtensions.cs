using System.Collections.Immutable;

namespace Timer.WorkoutTracking.Sound
{
    internal static class SoundEffectExtensions
    {
        public static ISoundEffect Then(this ISoundEffect first, ISoundEffect second)
        {
            return new SequenceOfSoundEffects(ImmutableArray.Create(first, second));
        }
    }
}