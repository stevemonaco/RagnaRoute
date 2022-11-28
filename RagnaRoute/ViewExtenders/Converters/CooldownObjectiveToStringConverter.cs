using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using NodaTime;
using RagnaRoute.Objectives;

namespace RagnaRoute.Converters;
public sealed class CooldownObjectiveToStringConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Count != 4)
            return AvaloniaProperty.UnsetValue;

        if (values[0] is Duration starting && values[1] is Duration ending && values[2] is Duration timeSinceStarted && values[3] is TimeState state)
        {
            return state switch
            {
                TimeState.Active => $"Active for {TimeHelpers.GetStringDuration(timeSinceStarted)}",
                TimeState.AwaitingUpcoming => $"Starting in {TimeHelpers.GetStringDuration(starting)} - {TimeHelpers.GetStringDuration(ending)}",
                TimeState.MaybeActive => $"Starting in {TimeHelpers.GetStringDuration(ending)}",
                TimeState.Inactive => "Inactive",
                TimeState.Ended => $"Ended",
                _ => throw new ArgumentException()
            };
        }

        return AvaloniaProperty.UnsetValue;
    }
}
