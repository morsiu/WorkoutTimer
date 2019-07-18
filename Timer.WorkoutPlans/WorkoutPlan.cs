using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlan
    {
        private readonly Duration _warmupDuration = Duration.FromSeconds(3);
        private readonly WorkoutRound _workoutRound;
        private readonly Count _roundCount;

        public WorkoutPlan(WorkoutRound workoutRound, Count roundCount)
        {
            _workoutRound = workoutRound;
            _roundCount = roundCount;
        }

        public IEnumerable<(Round Round, IEnumerable<T> Workouts)> Rounds<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break)
        {
            foreach (var round in
                _roundCount.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                yield return (round, WorkoutsOfRound(round));
            }

            IEnumerable<T> WorkoutsOfRound(Round round)
            {
                if (round.IsFirst)
                {
                    yield return warmUp(_warmupDuration);
                }

                foreach (var a in
                    _workoutRound.Select(
                        x => exercise(round, x),
                        x => @break(round, x)))
                {
                    yield return a;
                }
            }
        }

        [Pure]
        public IEnumerable<T> Workouts<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break,
            Func<Round, T> nonLastRoundDone,
            Func<Round, T> lastRoundDone)
        {
            yield return warmUp(_warmupDuration);
            foreach (var round in _roundCount.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                foreach (var x in 
                    _workoutRound.Select(
                        x => exercise(round, x),
                        x => @break(round, x)))
                {
                    yield return x;
                }
                if (!round.IsLast)
                {
                    yield return nonLastRoundDone(round);
                }
                else
                {
                    yield return lastRoundDone(round);
                }
            }
        }
    }
}
