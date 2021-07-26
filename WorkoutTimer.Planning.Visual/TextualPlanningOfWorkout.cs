using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkoutTimer.Plans;
using WorkoutTimer.Visual;

namespace WorkoutTimer.Planning.Visual
{
    public sealed class TextualPlanningOfWorkout : INotifyPropertyChanged
    {
        private readonly DelegateCommand _start;
        private readonly TaskCompletionSource<WorkoutPlan> _started;
        private string? _expression;
        private string? _parsedExpression;
        private WorkoutPlan? _parsedWorkoutPlan;

        public TextualPlanningOfWorkout(string? initialExpression)
        {
            var started = new TaskCompletionSource<WorkoutPlan>();
            var startCommand =
                new DelegateCommand(
                    () =>
                    {
                        if (ParsedWorkoutPlan is not null)
                            started.TrySetResult(ParsedWorkoutPlan);
                    },
                    () => ParsedWorkoutPlan != null);
            _started = started;
            _start = startCommand;
            Start = new OneOffCommand(startCommand);
            Expression = initialExpression;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                PropertyChanged?.Invoke(this);
                ParsedWorkoutPlan =
                    !string.IsNullOrWhiteSpace(_expression)
                        ? new WorkoutPlanExpression(_expression).ToWorkoutPlan(new WorkoutPlan())
                        : null;
                ParsedExpression = ParsedExpressionFromWorkoutPlan();
            }
        }

        public Task<WorkoutPlan> Finished => _started.Task;

        public string? ParsedExpression
        {
            get => _parsedExpression;
            set
            {
                _parsedExpression = value;
                PropertyChanged?.Invoke(this);
            }
        }

        public WorkoutPlan? ParsedWorkoutPlan
        {
            get => _parsedWorkoutPlan;
            set
            {
                _parsedWorkoutPlan = value;
                _start.RaiseCanExecuteChanged();
                PropertyChanged?.Invoke(this);
            }
        }

        public ICommand Start { get; }

        private string? ParsedExpressionFromWorkoutPlan()
        {
            if (ParsedWorkoutPlan is null) return null;
            var definition = ParsedWorkoutPlan
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
}