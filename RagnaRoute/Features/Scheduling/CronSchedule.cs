using System;
using NodaTime;
using Cronos;

namespace RagnaRoute.Scheduling;
public class CronSchedule : IScheduleIterable
{
    private readonly CronExpression _cronExpression;
    private readonly TimeZoneInfo _zone; // .NET-style TimeZoneInfo needed by Cronos
    private readonly Duration _duration;

    private static Duration[] _deltas = new Duration[]
    {
            Duration.FromHours(1), Duration.FromHours(12), Duration.FromDays(1),
            Duration.FromDays(3), Duration.FromDays(7), Duration.FromDays(14),
            Duration.FromDays(30), Duration.FromDays(90), Duration.FromDays(180),
            Duration.FromDays(365), Duration.FromDays(3650)
    };

    public CronSchedule(CronExpression cronExpression, DateTimeZone zone, Duration duration)
    {
        _cronExpression = cronExpression;
        _duration = duration;

        _zone = TimeZoneInfo.FindSystemTimeZoneById(zone.Id); // IANA supported on .NET6+
    }

    public Interval? Next(Instant instant)
    {
        var nextStart = _cronExpression.GetNextOccurrence(instant.ToDateTimeOffset(), _zone, true);

        if (nextStart is not null)
        {
            var startInstant = Instant.FromDateTimeOffset(nextStart.Value);
            return new Interval(startInstant, startInstant + _duration);
        }

        return null;
    }

    public Interval? Previous(Instant instant)
    {
        var currentNext = Next(instant);

        Interval? anyPreviousNext = null;

        foreach (var delta in _deltas)
        {
            anyPreviousNext = Next(instant - delta);

            if (anyPreviousNext!.Value.Start < currentNext!.Value.Start)
                break;
        }

        if (anyPreviousNext!.Value.Start == currentNext!.Value.Start)
            throw new ArgumentOutOfRangeException("Could not calculate a prior interval");

        Interval? previousNext = anyPreviousNext;
        Interval? nextVisitor;
        while (true)
        {
            nextVisitor = Next(previousNext.Value.Start + Duration.FromSeconds(1));

            if (nextVisitor is null)
                throw new ArgumentOutOfRangeException("Scheduler ran out of future objectives before finding a prior interval");

            if (nextVisitor.Value.Start == currentNext.Value.Start)
                return new Interval(previousNext.Value.Start, previousNext.Value.Start + _duration);

            previousNext = nextVisitor;
        }
    }
}
