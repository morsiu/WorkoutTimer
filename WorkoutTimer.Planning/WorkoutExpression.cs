using System;
using System.ComponentModel;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Planning
{
    public sealed class WorkoutExpression : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private enum FieldType
        {
            Break,
            Exercise,
            Countdown,
            Round,
            Repeat
        }

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan
        {
            get
            {
                return result =>
                {
                    var parts = Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    using var part = parts.AsEnumerable().GetEnumerator();
                    var previousAction = default(Func<WorkoutPlan, WorkoutPlan>);
                    while (part.MoveNext())
                    {
                        if (Value(part.Current) is { } value
                            && part.MoveNext())
                        {
                            if (Type(part.Current) is { } type)
                            {
                                if (Action(value, type, previousAction) is { } action)
                                {
                                    result = action(result);
                                    previousAction = action;
                                    continue;
                                }
                                return result;
                            }
                            return result;
                        }
                        return result;

                        static Func<WorkoutPlan, WorkoutPlan> Action(
                            int value,
                            FieldType type,
                            Func<WorkoutPlan, WorkoutPlan> previousAction)
                        {
                            if (Duration.TryFromSeconds(value) is { } duration)
                            {
                                switch (type)
                                {
                                    case FieldType.Break:
                                        return x => x.AddBreak(duration);
                                    case FieldType.Exercise:
                                        return x => x.AddExercise(duration);
                                    case FieldType.Countdown:
                                        return x => x.WithCountdown(duration);
                                }
                            }
                            if (value == 0 && type == FieldType.Exercise)
                            {
                                return x => x.AddExercise();
                            }
                            if (Count.TryFromNumber(value) is { } count)
                            {
                                switch (type)
                                {
                                    case FieldType.Round:
                                        return x => x.WithRound(count);
                                    case FieldType.Repeat when previousAction != null:
                                        return x => Enumerable.Repeat(previousAction, count - 1)
                                            .Aggregate(x, (a, b) => b(a));
                                }
                            }
                            return null;
                        }

                        static int? Value(string input)
                        {
                            return int.TryParse(input, out var number)
                                ? number
                                : default;
                        }

                        static FieldType? Type(string input)
                        {
                            return input.ToLower() switch
                            {
                                "e" => FieldType.Exercise,
                                "b" => FieldType.Break,
                                "w" => FieldType.Countdown,
                                "r" => FieldType.Round,
                                "*" => FieldType.Repeat,
                                _ => null
                            };
                        }
                    }

                    return result;
                };
            }
        }

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