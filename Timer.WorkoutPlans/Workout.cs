using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public sealed class Workout
    {
        private readonly WorkoutRound _workoutRound;
        private readonly RoundCount _roundCount;

        public Workout(WorkoutRound workoutRound, RoundCount roundCount)
        {
            _workoutRound = workoutRound;
            _roundCount = roundCount;
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
            foreach (var round in _roundCount.Rounds())
            {
                foreach (var x in _workoutRound.Select(exercise, @break))
                {
                    yield return x;
                }
                if (!round.IsLast)
                {
                    yield return roundDone();
                }
            }
            yield return workoutDone();
        }
    }
}
