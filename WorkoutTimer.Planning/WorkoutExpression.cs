using System;
using System.ComponentModel;
using System.Linq;
using WorkoutTimer.Planning.Textual;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Planning
{
    public sealed class WorkoutExpression : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan => new WorkoutPlanExpression(Value).ToWorkoutPlan;

        public string ActualValue
        {
            get
            {
                var definition = WorkoutPlan(new WorkoutPlan())
                    .Definition(
                        x => x is { } duration
                            ? $"{duration.TotalSeconds} W"
                            : string.Empty,
                        x => $"{x.TotalSeconds} E",
                        () => "E",
                        x => $"{x.TotalSeconds} B");
                return string.Format(
                    "{0} {1} R ({2})",
                    definition.Warmup,
                    definition.Round.Number,
                    definition.Round.Workouts.Any()
                        ? string.Join(" ", definition.Round.Workouts)
                        : "empty");
            }
        }

        public string DurationStatistics
        {
            get
            {
                var workoutPlan = WorkoutPlan(new WorkoutPlan());
                var workoutDurationStatistics = new WorkoutDurationStatistics(workoutPlan);
                return $"Total: {workoutDurationStatistics.Total()}, Exercise per round: {workoutDurationStatistics.ExercisePerRound()}";
            }
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkoutPlan)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DurationStatistics)));
            }
        }
    }
}