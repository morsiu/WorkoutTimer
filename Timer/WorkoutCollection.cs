using System.Collections.ObjectModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutCollection : Collection<Workout>
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