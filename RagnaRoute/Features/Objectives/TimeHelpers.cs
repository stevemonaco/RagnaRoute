using System;
using NodaTime;

namespace RagnaRoute.Objectives;

internal static class TimeHelpers
{
    public static TimeState DetermineTimeState(Duration timeUntilStarting, Duration timeUntilEnding)
    {
        if (timeUntilStarting < Duration.Zero && timeUntilEnding < Duration.Zero)
            return TimeState.After;
        else if (timeUntilStarting <= Duration.Zero && timeUntilEnding > Duration.Zero)
            return TimeState.During;
        else if (timeUntilStarting > Duration.Zero && timeUntilEnding >= Duration.Zero)
            return TimeState.Before;
        else
            throw new InvalidOperationException($"{nameof(DetermineTimeState)} with a positive {nameof(timeUntilStarting)} '{timeUntilStarting}' and negative {nameof(timeUntilEnding)} '{timeUntilEnding}'");
    }

    //public static ZonedDateTime ToZonedDateTime(this Instant instant)
    //{

    //}

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
            < -84000 => $"-{duration.Days}d{duration.Hours}h",
            < -3600 => $"-{duration.Hours}h{duration.Minutes}m",
            < -60 => $"-{duration.Minutes}m",
            _ => $"{duration.Seconds}s"
        };
    }
}
