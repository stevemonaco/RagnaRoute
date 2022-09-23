using NodaTime;
using RagnaRoute.Objectives.Extensions;
using RagnaRoute.Scheduling;

namespace RagnaRoute.Objectives;
public class ScheduledObjective : ITimedObjective
{
    public Interval? Prior { get; protected set; }
    public Interval? Ongoing { get; protected set; }
    public Interval? Upcoming { get; protected set; }
    public TimeState State { get; protected set; }
    public ObjectiveResult? PriorResult { get; protected set; }
    public Instant? LastCompletion { get; protected set; }

    private readonly IScheduleIterable _schedulable;
    private readonly IClock _clock;

    public ScheduledObjective(IScheduleIterable schedulable, Instant? lastCompletion = null, IClock? clock = null)
    {
        _schedulable = schedulable;
        _clock = clock ?? SystemClock.Instance;
        LastCompletion = lastCompletion;

        InitializeState();
    }

    public void InitializeState()
    {
        var current = _clock.GetCurrentInstant();
        var next = _schedulable.Next(current);
        var previous = _schedulable.Previous(current);

        if (current.IsWithin(previous))
        {
            if (LastCompletion?.IsWithin(previous) is true)
            {
                State = TimeState.AwaitingUpcoming;
                Upcoming = next;
            }
            else
            {
                Ongoing = previous;
                State = TimeState.Active;
            }
        }
        else
        {
            State = TimeState.AwaitingUpcoming;
            Upcoming = next;
        }

        if (LastCompletion?.IsWithin(previous) is true)
        {
            Prior = previous;
            PriorResult = ObjectiveResult.Completed;
        }
        else if (LastCompletion is not null)
        {
            PriorResult = ObjectiveResult.Missed;
        }
    }

    public bool Complete()
    {
        if (!(State == TimeState.Active || State == TimeState.MaybeActive))
            return false;

        var current = _clock.GetCurrentInstant();

        LastCompletion = current;
        Prior = Ongoing;
        Ongoing = null;
        Upcoming = _schedulable.Next(current);
        State = TimeState.AwaitingUpcoming;
        PriorResult = ObjectiveResult.Completed;

        return true;
    }

    public bool Update(Instant current)
    {
        var previousState = State;

        if (State == TimeState.AwaitingUpcoming && current.IsWithin(Upcoming))
        {
            Prior = Ongoing;
            Ongoing = Upcoming;
            State = TimeState.Active;
        }
        else if (State == TimeState.Active && current.IsAfter(Ongoing))
        {
            Upcoming = _schedulable.Next(current);
            PriorResult = ObjectiveResult.Missed;

            if (Upcoming is null)
                State = TimeState.Ended;
            else
                State = TimeState.Inactive;
        }

        return State == previousState;
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
