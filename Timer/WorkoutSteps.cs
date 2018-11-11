using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutSteps : Collection<WorkoutStep>
    {
        public IEnumerable<IWorkoutStep> ToWorkoutSteps()
        {
            var steps = this
                .Select(x => x.ToWorkoutStep())
                .Where(x => x != null)
                .ToList();
            return steps;
        }
    }
}