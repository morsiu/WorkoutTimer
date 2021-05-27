using System;

namespace WorkoutTimer.Tracking.Visual
{
    public sealed class RoundChangedEventArgs : EventArgs
    {
        public int? Round { get; }

        public RoundChangedEventArgs(int? round)
        {
            Round = round;
        }
    }
}