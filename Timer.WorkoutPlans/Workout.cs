using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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
            foreach (var roundNumber in Enumerable.Range(1, _roundCount))
            {
                foreach (var x in _workoutRound.Select(exercise, @break))
                {
                    yield return x;
                }

                if (roundNumber != _roundCount)
                {
                    yield return roundDone();
                }
            }

            yield return workoutDone();
        }
    }
}
