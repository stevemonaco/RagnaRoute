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
