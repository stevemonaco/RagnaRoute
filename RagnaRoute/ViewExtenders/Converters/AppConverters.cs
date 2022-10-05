using Avalonia.Data.Converters;
using NodaTime;
using RagnaRoute.Objectives;
using System.Globalization;

namespace RagnaRoute.Converters;
public static class AppConverters
{
    public static readonly IValueConverter DurationToString =
        new FuncValueConverter<Duration, string>(duration => TimeHelpers.GetStringDuration(duration));

    public static readonly IValueConverter TimeStateToEnabled =
        new FuncValueConverter<TimeState, bool>(x => x == TimeState.Active || x == TimeState.MaybeActive || x == TimeState.Inactive);

    public static readonly IValueConverter InstantToLocalString =
        new FuncValueConverter<Instant, string>(x =>
        {
            var culture = CultureInfo.CurrentCulture;

            var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var time = x.InZone(zone);
            return time.ToString(culture.DateTimeFormat.FullDateTimePattern, culture);
        });
}
