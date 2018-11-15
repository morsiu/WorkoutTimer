using System;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public readonly struct Duration : IEquatable<Duration>, IComparable<Duration>
    {
        public Duration(int seconds)
        {
            if (seconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "The value must not be negative.");
            }
            TotalSeconds = seconds;
        }

        public int TotalSeconds { get; }

        public static bool operator <(Duration left, Duration right) => left.CompareTo(right) < 0;

        public static bool operator >(Duration left, Duration right) => left.CompareTo(right) > 0;

        public static bool operator <=(Duration left, Duration right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Duration left, Duration right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Duration left, Duration right) => left.CompareTo(right) == 0;

        public static bool operator !=(Duration left, Duration right) => left.CompareTo(right) != 0;

        public static Duration FromSeconds(int seconds) => new Duration(seconds);

        public static Duration? TryFromSeconds(int seconds) =>
            seconds >= 0
                ? new Duration(seconds)
                : default(Duration?);

        public int CompareTo(Duration other) => TotalSeconds.CompareTo(other.TotalSeconds);

        public override bool Equals(object obj) => obj is Duration other && Equals(other);

        public bool Equals(Duration other) => CompareTo(other) == 0;

        public override int GetHashCode() => TotalSeconds.GetHashCode();

        public override string ToString() => TotalSeconds.ToString();

        [Pure] public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(TotalSeconds);
    }
}