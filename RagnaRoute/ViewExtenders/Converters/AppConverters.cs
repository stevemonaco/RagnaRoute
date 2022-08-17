using Avalonia.Data.Converters;
using NodaTime;

namespace RagnaRoute.Converters;
public static class AppConverters
{
    public static readonly IValueConverter DurationToString =
        new FuncValueConverter<Duration, string>(duration => duration.TotalSeconds switch
            {
                > 86400 => $"{duration.Days}d{duration.Hours}h",
                > 3600 => $"{duration.Hours}h{duration.Minutes}m",
                > 60 => $"{duration.Minutes}m",
                _ => $"{duration.Seconds}s"
            });
}
