using System.Collections.Generic;

namespace Timer.WorkoutPlans
{
    public readonly struct Workout
    {
        public Workout(IEnumerable<IWorkoutStep> steps, SetCount setCount)
        {
            Steps = steps;
            SetCount = setCount;
        }

        public IEnumerable<IWorkoutStep> Steps { get; }

        public SetCount SetCount { get; }
    }
}
