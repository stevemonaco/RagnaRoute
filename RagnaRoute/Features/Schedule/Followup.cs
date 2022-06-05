using NodaTime;
using RagnaRoute.Objectives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Features.Schedule;
public class Followup : IFollowup
{
    private readonly Func<Instant, Interval> _nextFollowup;

    private Followup(Func<Instant, Interval> nextFollowup)
    {
        _nextFollowup = nextFollowup;
    }

    public Interval Next(Instant current)
    {
        return _nextFollowup(current);
    }

    public static IFollowup OnWeekly(IsoDayOfWeek scheduledDay, LocalTime scheduledTime, Duration duration)
    {
        Func<Instant, Interval> next = (current) =>
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var currentZonedTime = current.InZone(timezone);

            var scheduledDateTime = new LocalDateTime(currentZonedTime.Year, currentZonedTime.Month, (int)scheduledDay, scheduledTime.Hour, scheduledTime.Minute);
            var endScheduledDateTime = scheduledDateTime.PlusSeconds((int)duration.TotalSeconds);

            if (currentZonedTime.LocalDateTime > endScheduledDateTime)
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledDateTime.PlusWeeks(1), endScheduledDateTime.PlusWeeks(1), timezone);
            }
            else
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledDateTime, endScheduledDateTime, timezone);
            }
        };

        return new Followup(next);
    }

    public static IFollowup OnDaily(LocalTime scheduledTime, Duration duration)
    {
        Interval Next(Instant current)
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var currentZonedTime = current.InZone(timezone);

            var scheduledDateTime = new LocalDateTime(currentZonedTime.Year, currentZonedTime.Month, currentZonedTime.Day, scheduledTime.Hour, scheduledTime.Minute);
            var endScheduledDateTime = scheduledDateTime.PlusSeconds((int)duration.TotalSeconds);

            if (currentZonedTime.LocalDateTime > endScheduledDateTime)
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledDateTime.PlusDays(1), endScheduledDateTime.PlusDays(1), timezone);
            }
            else
            {
                return TimeHelpers.LocalDateTimesToInterval(scheduledDateTime, endScheduledDateTime, timezone);
            }
        }

        return new Followup(Next);
    }

    public static IFollowup OnSchedule(Func<Instant, Interval> nextFollowup) => new Followup(nextFollowup);
}
