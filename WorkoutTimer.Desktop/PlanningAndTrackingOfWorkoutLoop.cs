using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTimer.Tracking;
using WorkoutTimer.Tracking.Sound;
using WorkoutTimer.Tracking.Sound.NAudio;
using WorkoutTimer.Tracking.Visual;
using WorkoutTimer.Visual;

namespace WorkoutTimer.Desktop
{
    internal sealed class PlanningAndTrackingOfWorkoutLoop
    {
        public static SwitchedViewModel Create()
        {
            var textualPlanningAndStatistics = new TextualPlanningAndStatisticsOfWorkout(initialExpression: null);
            return new SwitchedViewModel(textualPlanningAndStatistics, ViewModels(textualPlanningAndStatistics));

            static async IAsyncEnumerable<object> ViewModels(
                TextualPlanningAndStatisticsOfWorkout textualPlanningAndStatistics)
            {
                using var soundFactory = new NAudioSoundFactory();
                while (true)
                {
                    {
                        var workoutPlan = await textualPlanningAndStatistics.Planning.Finished;
                        var trackedWorkoutPlan = new TrackedWorkoutPlan(workoutPlan);
                        var trackingCancellation = new CancellationTokenSource();
                        var visualTracking =
                            new VisualTrackingOfWorkout(trackedWorkoutPlan, workoutPlan, trackingCancellation);
                        var soundTracking = new SoundTrackingOfWorkout(trackedWorkoutPlan, soundFactory);
                        yield return visualTracking;
                        try
                        {
                            await trackedWorkoutPlan.Start(trackingCancellation.Token);
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        finally
                        {
                            visualTracking.Dispose();
                            _ = soundTracking.DisposeAsync();
                        }
                    }
                    textualPlanningAndStatistics =
                        new TextualPlanningAndStatisticsOfWorkout(textualPlanningAndStatistics.Planning.Expression);
                    yield return textualPlanningAndStatistics;
                }
            }
        }
    }
}