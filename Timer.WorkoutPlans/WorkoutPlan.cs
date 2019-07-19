using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Timer.WorkoutPlans
{
    public sealed class WorkoutPlan
    {
        private readonly Duration _warmup;
        private readonly WorkoutRound _workoutRound;
        private readonly Count _rounds;

        public WorkoutPlan()
            : this(new WorkoutRound(), new Count(1), new Duration(0))
        {
        }

        private WorkoutPlan(WorkoutRound workoutRound, Count rounds, Duration warmup)
        {
            _workoutRound = workoutRound;
            _rounds = rounds;
            _warmup = warmup;
        }

        public WorkoutPlan AddBreak(Duration duration)
        {
            return new WorkoutPlan(
                _workoutRound.AddBreakWorkout(duration),
                _rounds,
                _warmup);
        }

        public WorkoutPlan AddExercise(Duration duration)
        {
            return new WorkoutPlan(
                _workoutRound.AddExerciseWorkout(duration),
                _rounds,
                _warmup);
        }

        public IEnumerable<(Round Round, IEnumerable<T> Workouts)> Rounds<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break)
        {
            foreach (var round in
                _rounds.Enumerate(x => new Round(x.Number, x.IsLast)))
            {
                yield return (round, WorkoutsOfRound(round));
            }

            IEnumerable<T> WorkoutsOfRound(Round round)
            {
                if (round.IsFirst)
                {
                    yield return warmUp(_warmup);
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

        public IEnumerable<T> Round<T>(
            Func<Duration, T> exercise,
            Func<Duration, T> @break)
        {
            return _workoutRound.Select(exercise, @break);
        }

        [Pure]
        public IEnumerable<T> Workouts<T>(
            Func<Duration, T> warmUp,
            Func<Round, Duration, T> exercise,
            Func<Round, Duration, T> @break,
            Func<Round, T> nonLastRoundDone,
            Func<Round, T> lastRoundDone)
        {
            yield return warmUp(_warmup);
            foreach (var round in _rounds.Enumerate(x => new Round(x.Number, x.IsLast)))
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

        public WorkoutPlan WithCountdown(Duration value)
        {
            return new WorkoutPlan(
                _workoutRound,
                _rounds,
                warmup: value);
        }

        public WorkoutPlan WithRound(Count value)
        {
            return new WorkoutPlan(
                _workoutRound,
                rounds: value,
                _warmup);
        }
    }
}
