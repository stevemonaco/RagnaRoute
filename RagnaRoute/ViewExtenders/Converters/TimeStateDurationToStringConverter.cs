using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using NodaTime;
using RagnaRoute.Objectives;

namespace RagnaRoute.Converters;
internal class TimeStateDurationToStringConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Count != 3)
            return AvaloniaProperty.UnsetValue;

        if (values[0] is Duration starting && values[1] is Duration ending && values[2] is TimeState state)
        {
            return state switch
            {
                TimeState.Before => $"{TimeHelpers.GetStringDuration(starting)} - {TimeHelpers.GetStringDuration(ending)}",
                TimeState.During => TimeHelpers.GetStringDuration(ending),
                TimeState.After => TimeHelpers.GetStringDuration(ending),
                TimeState.Completed => " ",
                TimeState.Indeterminate => " ",
                _ => throw new ArgumentException()
            };
        }

        return AvaloniaProperty.UnsetValue;
    }
}
