using System;
using System.Linq;
using WorkoutTimer.Plans;

namespace WorkoutTimer.Planning
{
    public sealed class WorkoutPlanExpression
    {
        private readonly string _expression;

        public WorkoutPlanExpression(string expression)
        {
            _expression = expression;
        }

        public WorkoutPlan ToWorkoutPlan(WorkoutPlan result)
        {
            var parts = _expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            using var part = parts.AsEnumerable().GetEnumerator();
            var previousAction = default(Func<WorkoutPlan, WorkoutPlan>?);
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
                if (Type(part.Current) is FieldType.Exercise)
                {
                    result = result.AddExercise();
                    previousAction = x => x.AddExercise();
                    continue;
                }
                return result;

                static Func<WorkoutPlan, WorkoutPlan>? Action(
                    int value,
                    FieldType type,
                    Func<WorkoutPlan, WorkoutPlan>? previousAction)
                {
                    if (Duration.TryFromSeconds(value) is { } duration)
                        switch (type)
                        {
                            case FieldType.Break:
                                return x => x.AddBreak(duration);
                            case FieldType.Exercise:
                                return x => x.AddExercise(duration);
                            case FieldType.Countdown:
                                return x => x.WithCountdown(duration);
                        }
                    if (Count.TryFromNumber(value) is { } count)
                        switch (type)
                        {
                            case FieldType.Round:
                                return x => x.WithRound(count);
                            case FieldType.Repeat when previousAction != null:
                                return x => Enumerable.Repeat(previousAction, count - 1)
                                    .Aggregate(x, (a, b) => b(a));
                        }
                    return null;
                }

                static int? Value(string input)
                {
                    return int.TryParse(input, out var number)
                        ? number
                        : default(int?);
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
        }

        private enum FieldType
        {
            Break,
            Exercise,
            Countdown,
            Round,
            Repeat
        }
    }
}