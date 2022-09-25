using System;
using Cronos;
using NodaTime;
using RagnaRoute.Scheduling;

namespace RagnaRoute.Objectives;

internal static class TimeHelpers
{
    /// <summary>
    /// Constructs an Interval from two LocalDateTime using the system default time zone
    /// </summary>
    /// <param name="start">Start of the Interval</param>
    /// <param name="end">End of the Interval</param>
    /// <returns></returns>
    public static Interval LocalDateTimesToInterval(LocalDateTime start, LocalDateTime end)
    {
        var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

        return LocalDateTimesToInterval(start, end, zone);
    }

    /// <summary>
    /// Constructs an Interval from two LocalDateTime using the specified time zone
    /// </summary>
    /// <param name="start">Start of the Interval</param>
    /// <param name="end">End of the Interval</param>
    /// <param name="zone">Time zone to map with</param>
    /// <returns></returns>
    public static Interval LocalDateTimesToInterval(LocalDateTime start, LocalDateTime end, DateTimeZone zone)
    {
        return new Interval(start.InZoneLeniently(zone).ToInstant(), end.InZoneLeniently(zone).ToInstant());
    }

    public static string GetStringDuration(Duration duration)
    {
        return duration.TotalSeconds switch
        {
            > 86400 => $"{duration.Days}d{duration.Hours}h",
            > 3600 => $"{duration.Hours}h{duration.Minutes}m",
            > 60 => $"{duration.Minutes}m",
            < -84000 => $"-{Math.Abs(duration.Days)}d{Math.Abs(duration.Hours)}h",
            < -3600 => $"-{Math.Abs(duration.Hours)}h{Math.Abs(duration.Minutes)}m",
            < -60 => $"-{Math.Abs(duration.Minutes)}m",
            _ => $"{duration.Seconds}s"
        };
    }

    private static Duration[] _deltas = new Duration[]
    {
            Duration.FromHours(1), Duration.FromHours(12), Duration.FromDays(1),
            Duration.FromDays(3), Duration.FromDays(7), Duration.FromDays(14),
            Duration.FromDays(30), Duration.FromDays(90), Duration.FromDays(180),
            Duration.FromDays(365), Duration.FromDays(3650)
    };
}
