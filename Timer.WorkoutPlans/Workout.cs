using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Timer.WorkoutPlans
{
    public readonly struct Workout
    {
        private readonly IEnumerable<IWorkoutStep> _steps;

        public Workout(IEnumerable<IWorkoutStep> steps, SetCount setCount)
        {
            _steps = steps;
            SetCount = setCount;
        }

        [Pure]
        public IEnumerable<IWorkoutStep> Steps()
        {
            yield return new Warmup();
            foreach (var set in Enumerable.Range(1, SetCount))
            {
                foreach (var step in _steps)
                {
                    yield return step;
                }

                if (set != SetCount)
                {
                    yield return new SetDone();
                }
            }

            yield return new WorkoutDone();
        }

        public SetCount SetCount { get; }
    }
}
