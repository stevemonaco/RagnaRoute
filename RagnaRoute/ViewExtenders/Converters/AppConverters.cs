using Avalonia.Data.Converters;
using NodaTime;
using RagnaRoute.Objectives;

namespace RagnaRoute.Converters;
public static class AppConverters
{
    public static readonly IValueConverter DurationToString =
        new FuncValueConverter<Duration, string>(duration => TimeHelpers.GetStringDuration(duration));
}
