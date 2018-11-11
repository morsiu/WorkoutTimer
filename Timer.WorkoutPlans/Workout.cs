using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Timer.WorkoutPlans
{
    public sealed class Workout
    {
        private readonly WorkoutRound _workoutRound;
        private readonly SetCount _setCount;

        public Workout(WorkoutRound workoutRound, SetCount setCount)
        {
            _workoutRound = workoutRound;
            _setCount = setCount;
        }

        [Pure]
        public IEnumerable<T> Select<T>(
            Func<Duration, T> warmUp,
            Func<Duration, T> exercise,
            Func<Duration, T> @break,
            Func<T> roundDone,
            Func<T> workoutDone)
        {
            yield return warmUp(Duration.FromSeconds(15));
            foreach (var set in Enumerable.Range(1, _setCount))
            {
                foreach (var x in _workoutRound.Select(exercise, @break))
                {
                    yield return x;
                }

                if (set != _setCount)
                {
                    yield return roundDone();
                }
            }

            yield return workoutDone();
        }
    }
}
