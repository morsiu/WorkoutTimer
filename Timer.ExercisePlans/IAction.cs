using System;

namespace Timer.ExercisePlans
{
    public interface IAction
    {
        T Map<T>(
            Func<TimeSpan, T> exercise,
            Func<TimeSpan, T> @break,
            Func<T> invalid);
    }
}