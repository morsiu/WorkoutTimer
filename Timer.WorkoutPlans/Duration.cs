using System;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public readonly struct Duration : IEquatable<Duration>, IComparable<Duration>
    {
        private readonly int _secondsMinusOne;

        public Duration(int seconds)
        {
            if (seconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "The value must be greater than zero.");
            }
            _secondsMinusOne = seconds - 1;
        }

        public int TotalSeconds => _secondsMinusOne + 1;

        public static Duration? TryFromSeconds(int seconds) =>
            seconds > 0
                ? new Duration(seconds)
                : default(Duration?);

        public Duration Add(in Duration duration) =>
            new(TotalSeconds + duration.TotalSeconds);

        public int CompareTo(Duration other) => _secondsMinusOne.CompareTo(other._secondsMinusOne);

        public override bool Equals(object obj) => obj is Duration other && Equals(other);

        public bool Equals(Duration other) => CompareTo(other) == 0;

        public override int GetHashCode() => _secondsMinusOne.GetHashCode();

        public override string ToString() => TotalSeconds.ToString();

        [Pure] public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(TotalSeconds);

        public static bool operator <(Duration left, Duration right) => left.CompareTo(right) < 0;

        public static bool operator >(Duration left, Duration right) => left.CompareTo(right) > 0;

        public static bool operator <=(Duration left, Duration right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Duration left, Duration right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Duration left, Duration right) => left.CompareTo(right) == 0;

        public static bool operator !=(Duration left, Duration right) => left.CompareTo(right) != 0;
    }
}