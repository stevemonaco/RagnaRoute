using System;
using NodaTime;
using RagnaRoute.Objectives;

namespace RagnaRoute.Scheduling;
public class Followup : IFollowup
{
    private readonly Func<Instant, Interval?> _nextFollowup;

    private Followup(Func<Instant, Interval?> nextFollowup)
    {
        _nextFollowup = nextFollowup;
    }

    public Interval? Next(Instant current)
    {
        return _nextFollowup(current);
    }

    public static IFollowup Never()
    {
        return new Followup(instant => null);
    }

    public static IFollowup OnWeekly(IsoDayOfWeek scheduledDay, LocalTime scheduledTime, Duration duration)
    {
        Interval? Next(Instant current)
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var currentZonedTime = current.InZone(timezone);

            var scheduledStart = new LocalDateTime(currentZonedTime.Year, currentZonedTime.Month, (int)scheduledDay, scheduledTime.Hour, scheduledTime.Minute);
            var scheduledEnd = scheduledStart.PlusSeconds((int)duration.TotalSeconds);

            if (currentZonedTime.LocalDateTime > scheduledEnd)
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledStart.PlusWeeks(1), scheduledEnd.PlusWeeks(1), timezone);
            }
            else
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledStart, scheduledEnd, timezone);
            }
        };

        return new Followup(Next);
    }

    public static IFollowup OnDaily(LocalTime scheduledTime, Duration duration)
    {
        Interval? Next(Instant current)
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var currentZonedTime = current.InZone(timezone);

            var scheduledStart = new LocalDateTime(currentZonedTime.Year, currentZonedTime.Month, currentZonedTime.Day, scheduledTime.Hour, scheduledTime.Minute);
            var scheduledEnd = scheduledStart.PlusSeconds((int)duration.TotalSeconds);

            if (currentZonedTime.LocalDateTime > scheduledEnd)
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledStart.PlusDays(1), scheduledEnd.PlusDays(1), timezone);
            }
            else
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledStart, scheduledEnd, timezone);
            }
        }

        return new Followup(Next);
    }

    public static IFollowup OnInterval(Duration duration) => OnInterval(duration, duration);

    public static IFollowup OnInterval(Duration minimum, Duration maximum)
    {
        Interval? Next(Instant current)
        {
            var start = current + minimum;
            var end = current + maximum;
            return new Interval(start, end);
        }

        return new Followup(Next);
    }

    public static IFollowup OnSchedule(Func<Instant, Interval?> nextFollowup) => new Followup(nextFollowup);
}
