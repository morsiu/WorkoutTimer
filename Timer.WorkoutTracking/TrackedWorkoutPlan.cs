using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking
{
    public sealed class TrackedWorkoutPlan
    {
        private readonly Dictionary<object, TrackedWorkoutPlanVisitor> _visitors = new();
        private readonly WorkoutPlan _workoutPlan;

        public TrackedWorkoutPlan(WorkoutPlan workoutPlan)
        {
            _workoutPlan = workoutPlan;
        }

        public IDisposable Subscribe(TrackedWorkoutPlanVisitor visitor)
        {
            var key = new object();
            _visitors.Add(key, visitor);
            return new Subscription(this, key);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            var allWorkouts = _workoutPlan.Enumerate(
                new WorkoutPlanVisitor<TrackedWorkout>()
                    .OnBreak((round, index, duration) => new Break(round, index, duration))
                    .OnExercise((round, index, duration) => new ExerciseWithDuration(round, index, duration))
                    .OnExercise((round, index) => new ExerciseWithoutDuration(round, index))
                    .OnWarmup(duration => new Warmup(duration)));
            foreach (var (round, workouts) in allWorkouts)
            {
                foreach (var visitor in _visitors.Values)
                {
                    visitor.VisitRoundStart(round, cancellationToken);
                }
                foreach (var workout in workouts)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    foreach (var visitor in _visitors.Values)
                    {
                        visitor.VisitWorkoutStart(workout, cancellationToken);
                    }
                    await workout.Track(cancellationToken);
                    foreach (var visitor in _visitors.Values)
                    {
                        visitor.VisitWorkoutEnd(workout, cancellationToken);
                    }
                }
                foreach (var visitor in _visitors.Values)
                {
                    visitor.VisitRoundEnd(round, cancellationToken);
                }
            }
        }

        private sealed class Subscription : IDisposable
        {
            private readonly object _key;
            private readonly WeakReference<TrackedWorkoutPlan> _owner;

            public Subscription(TrackedWorkoutPlan owner, object key) =>
                (_key, _owner) = (key, new(owner));

            public void Dispose()
            {
                if (_owner.TryGetTarget(out var owner))
                {
                    owner._visitors.Remove(_key);
                }
            }
        }
    }
}
