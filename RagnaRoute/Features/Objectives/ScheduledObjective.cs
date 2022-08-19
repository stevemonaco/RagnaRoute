using NodaTime;
using RagnaRoute.Scheduling;
using System;

namespace RagnaRoute.Objectives;
public class ScheduledObjective : IRecurringObjective
{
    public Instant Start { get; private set; }
    public Duration Duration { get; private set; }
    public Instant End => Start + Duration;

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }
    public TimeState State { get; private set; } = TimeState.Indeterminate;

    public Instant? LastCompletion { get; private set; }

    //private IFollowup _followup = Followup.OnDaily(new LocalTime(4, 0), Duration.FromDays(1));
    //private IFollowup _followup = Followup.OnSchedule(x => new Interval(x.Plus(Duration.FromSeconds(10)), x.Plus(Duration.FromSeconds(12))));
    private IFollowup _followup;
    private Instant? _lastUpdate;
    private Interval? _next;

    public ScheduledObjective(Instant start, IFollowup followup)
    {
        Start = start;
        _followup = followup;

        _next = _followup.Next(SystemClock.Instance.GetCurrentInstant());
    }

    public void Next()
    {
        throw new NotImplementedException();
    }

    public void Complete() =>
        Complete(SystemClock.Instance.GetCurrentInstant());

    public void Complete(Instant? resetTime)
    {
        LastCompletion = resetTime;

        if (_lastUpdate is not null)
            UpdateRemaining(_lastUpdate.Value);
    }

    public void Reset()
    {
        LastCompletion = null;
        State = TimeState.Indeterminate;
    }

    public void Update(Instant current)
    {
        UpdateRemaining(current);
        _lastUpdate = current;
    }

    private void UpdateRemaining(Instant current)
    {
        if (LastCompletion is null)
            return;

        var next = _followup.Next(current);

        if (_lastUpdate is not null && _lastUpdate < Start && current >= Start)
        {
            State = TimeState.During;
        }

        if (next is Interval)
        {
            TimeUntilStarting = next.Value.Start - current;
            TimeUntilEnding = next.Value.End - current;

            if (State != TimeState.Completed)
                State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
        }
        else
        {
            TimeUntilStarting = Duration.MaxValue;
            TimeUntilEnding = Duration.MaxValue;

            if (State != TimeState.Completed)
                State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
        }
    }
}
