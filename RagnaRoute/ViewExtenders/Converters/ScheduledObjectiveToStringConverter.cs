using Avalonia.Data.Converters;
using Avalonia;
using NodaTime;
using RagnaRoute.Objectives;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RagnaRoute.Converters;
internal class ScheduledObjectiveToStringConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Count != 3)
            return AvaloniaProperty.UnsetValue;

        if (values[0] is Duration starting && values[1] is Duration ending && values[2] is TimeState state)
        {
            return state switch
            {
                TimeState.AwaitingUpcoming when starting > Duration.Zero => $"{TimeHelpers.GetStringDuration(starting)} - {TimeHelpers.GetStringDuration(ending)}: Awaiting",
                TimeState.AwaitingUpcoming when starting <= Duration.Zero => $"{TimeHelpers.GetStringDuration(ending)}: Awaiting",
                TimeState.Active => $"{TimeHelpers.GetStringDuration(ending)} Remaining: Active",
                TimeState.MaybeActive => $"{TimeHelpers.GetStringDuration(ending)}: MaybeActive",
                TimeState.Inactive => "Inactive",
                TimeState.Ended => $"{TimeHelpers.GetStringDuration(ending)}: Ended",
                _ => throw new ArgumentException()
            };
        }

        return AvaloniaProperty.UnsetValue;
    }
}
