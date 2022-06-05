using NodaTime;
using RagnaRoute.Features.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Objectives;
public class ScheduledObjective : IRecurringObjective
{
    public Instant Start { get; private set; }
    public Duration Duration { get; private set; }
    public Instant End => Start + Duration;

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }
    public TimeState State { get; private set; } = TimeState.Indeterminate;

    public Instant? LastReset { get; private set; }

    private IFollowup _followup = Followup.OnDaily(new LocalTime(4, 0), Duration.FromDays(1));
    private Instant? _lastUpdate;

    public ScheduledObjective()
    {

    }

    public void Reset() =>
        Reset(SystemClock.Instance.GetCurrentInstant());

    public void Reset(Instant? resetTime)
    {
        LastReset = resetTime;

        if (_lastUpdate is not null)
            UpdateRemaining(_lastUpdate.Value);
    }

    public void Update(Instant current)
    {
        _lastUpdate = current;

        UpdateRemaining(current);
    }

    private void UpdateRemaining(Instant current)
    {
        if (LastReset is null)
            return;

        var next = _followup.Next(current);

        TimeUntilStarting = next.Start - current;
        TimeUntilEnding = next.End - current;

        State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
    }
}
