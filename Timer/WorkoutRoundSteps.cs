using System.Collections.ObjectModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutRoundSteps : Collection<WorkoutRoundStep>
    {
        public WorkoutRound ToWorkoutRound()
        {
            var round =
                this.Aggregate(
                    new WorkoutRound(),
                    (x, y) => y.AddTo(x));
            return round;
        }
    }
}