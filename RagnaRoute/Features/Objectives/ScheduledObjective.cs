using NodaTime;
using RagnaRoute.Scheduling;
using System;

namespace RagnaRoute.Objectives;

public class ScheduledObjective : ITimedObjective
{
    public Interval? Prior { get; }
    public Interval? Ongoing { get; }
    public Interval? Upcoming { get; }
    public TimeState State { get; }
    public ObjectiveResult? PriorResult { get; }
    public Instant? LastCompletion { get; }

    public ScheduledObjective(IScheduleIterable schedulable, Instant? lastCompletion = null)
    {

    }

    public bool Complete()
    {
        throw new NotImplementedException();
    }

    public bool Update(Instant current)
    {
        throw new NotImplementedException();
    }
}

//public class ScheduledObjective : IRecurringObjective
//{
//    public Instant Start { get; private set; }
//    public Duration Duration { get; private set; }
//    public Instant End => Start + Duration;

//    public Duration TimeUntilStarting { get; private set; }
//    public Duration TimeUntilEnding { get; private set; }
//    public TimeState State { get; private set; } = TimeState.Indeterminate;

//    public Instant? LastCompletion { get; private set; }

//    //private IFollowup _followup = Followup.OnDaily(new LocalTime(4, 0), Duration.FromDays(1));
//    //private IFollowup _followup = Followup.OnSchedule(x => new Interval(x.Plus(Duration.FromSeconds(10)), x.Plus(Duration.FromSeconds(12))));
//    private IScheduleIterable _schedulable;
//    private Instant? _lastUpdate;
//    private Interval? _next;

//    public ScheduledObjective(IScheduleIterable schedulable, Instant? lastCompletion = null)
//    {
//        LastCompletion = lastCompletion;

//        //Start = start;
//        _schedulable = schedulable;

//        if (lastCompletion.HasValue)
//        {
//            var completionNextStart = _schedulable.Next(lastCompletion.Value);
//            var currentNextStart = _schedulable.Next(SystemClock.Instance.GetCurrentInstant());

//            if (completionNextStart!.Value == currentNextStart!.Value) // Completed within the current schedule window
//            {
//                State = TimeState.Completed;
//                _next = currentNextStart.Value;
//            }
//            else
//            {
//                _next = currentNextStart.Value;
//            }

//            _next = _schedulable.Next(SystemClock.Instance.GetCurrentInstant());
//        }
//        else
//        {
//            _next = _schedulable.Next(SystemClock.Instance.GetCurrentInstant());
//        }
//    }

//    public void Skip()
//    {
//        var instant = SystemClock.Instance.GetCurrentInstant();
//        _next = _schedulable.Next(instant);
//        Update(instant);
//    }

//    public void Complete() =>
//        Complete(SystemClock.Instance.GetCurrentInstant());

//    public void Complete(Instant? resetTime)
//    {
//        LastCompletion = resetTime;

//        if (_lastUpdate is not null)
//            UpdateRemaining(_lastUpdate.Value);
//    }

//    public void Reset()
//    {
//        LastCompletion = null;
//        State = TimeState.Indeterminate;
//    }

//    public void Update(Instant current)
//    {
//        UpdateRemaining(current);
//        _lastUpdate = current;
//    }

//    private void UpdateRemaining(Instant current)
//    {
//        if (LastCompletion is null && _next is not null)
//        {
//            TimeUntilStarting = _next.Value.Start - current;
//            TimeUntilEnding = _next.Value.End - current;

//            State = TimeState.Before;
//        }
//        else
//        {
//            if (_lastUpdate is not null && _lastUpdate < Start && current >= Start)
//            {
//                State = TimeState.During;
//            }
//            else if (_next is not null)
//            {
//                TimeUntilStarting = _next.Value.Start - current;
//                TimeUntilEnding = _next.Value.End - current;

//                if (State != TimeState.Completed)
//                    State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
//            }
//            else
//            {
//                TimeUntilStarting = Duration.MaxValue;
//                TimeUntilEnding = Duration.MaxValue;

//                if (State != TimeState.Completed)
//                    State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
//            }
//        }
//    }
//}
