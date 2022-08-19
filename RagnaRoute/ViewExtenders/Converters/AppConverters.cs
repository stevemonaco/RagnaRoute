using Avalonia.Data.Converters;
using NodaTime;
using RagnaRoute.Objectives;
using RagnaRoute.ViewModels;
using System;

namespace RagnaRoute.Converters;
public static class AppConverters
{
    public static readonly IValueConverter DurationToString =
        new FuncValueConverter<Duration, string>(duration => TimeHelpers.GetStringDuration(duration));

    public static readonly IValueConverter BossToTimeString =
        new FuncValueConverter<BossQuestViewModel?, string>(boss =>
        {
            if (boss is null)
                return " ";

            return boss.TimeState switch
            {
                Objectives.TimeState.Before => $"{TimeHelpers.GetStringDuration(boss.TimeUntilStarting)} - {TimeHelpers.GetStringDuration(boss.TimeUntilEnding)}",
                Objectives.TimeState.During => TimeHelpers.GetStringDuration(boss.TimeUntilEnding),
                Objectives.TimeState.After => TimeHelpers.GetStringDuration(boss.TimeUntilEnding),
                Objectives.TimeState.Completed => " ",
                Objectives.TimeState.Indeterminate => " ",
                _ => throw new ArgumentException()
            };
        });
}
